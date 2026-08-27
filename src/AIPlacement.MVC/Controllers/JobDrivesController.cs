using AIPlacement.Application.Company.Interfaces;
using AIPlacement.Application.Jobs;
using AIPlacement.Application.Jobs.DTOs;
using AIPlacement.Application.Jobs.Interfaces;
using AIPlacement.Application.Skills.Interfaces;
using AIPlacement.MVC.Models.CompanyAndJob;
using AIPlacement.Application.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AIPlacement.MVC.Controllers;

[Authorize(Roles = RoleNames.Company)]
public class JobDrivesController : Controller
{
    private readonly ICompanyService _companyService;
    private readonly IJobDriveService _jobDriveService;
    private readonly ISkillService _skillService;

    public JobDrivesController(ICompanyService companyService,
        IJobDriveService jobDriveService, ISkillService skillService)
    {
        _companyService = companyService;
        _jobDriveService = jobDriveService;
        _skillService = skillService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? companyId, string? status,
        string? approvalStatus, string? search)
    {
        var company = await GetCompanyAsync(companyId);
        if (company.Result is not null) return company.Result;
        if (!IsValidStatus(status)) return BadRequest("Invalid JobDrive status filter.");
        if (!IsValidApprovalStatus(approvalStatus)) return BadRequest("Invalid approval status filter.");

        var jobs = await _jobDriveService.GetByCompanyIdAsync(company.Value!.CompanyId);
        var filtered = jobs
            .Where(job => string.IsNullOrWhiteSpace(status) ||
                job.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
            .Where(job => string.IsNullOrWhiteSpace(approvalStatus) ||
                job.ApprovalStatus.Equals(approvalStatus, StringComparison.OrdinalIgnoreCase))
            .Where(job => string.IsNullOrWhiteSpace(search) ||
                job.JobTitle.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                job.Location.Contains(search, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return View(new JobDriveListViewModel
        {
            CompanyId = company.Value.CompanyId,
            CompanyName = company.Value.CompanyName,
            SelectedStatus = status,
            SelectedApprovalStatus = approvalStatus,
            JobDrives = filtered
        });
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? companyId)
    {
        var company = await GetCompanyAsync(companyId);
        if (company.Result is not null) return company.Result;

        var model = new JobDriveFormViewModel
        {
            CompanyId = company.Value!.CompanyId,
            GraduationYear = DateTime.UtcNow.Year,
            ApplicationDeadline = DateTime.Now.AddDays(7)
        };
        await LoadFormChoicesAsync(model);
        return View("Create", model);
    }

    [HttpPost("/JobDrives/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(JobDriveFormViewModel model)
    {
        await ValidateFormAsync(model);
        if (!ModelState.IsValid)
        {
            await LoadFormChoicesAsync(model);
            return View("Create", model);
        }

        try
        {
            var created = await _jobDriveService.CreateAsync(ToCreateDto(model));
            TempData["SuccessMessage"] = $"JobDrive '{created.JobTitle}' was created.";
            return RedirectToAction(nameof(Index), new { companyId = model.CompanyId });
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await LoadFormChoicesAsync(model);
            return View("Create", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(int jobDriveId, int companyId)
    {
        var result = await GetOwnedJobAsync(jobDriveId, companyId);
        if (result.Error is not null) return result.Error;

        var skillIds = result.Job!.RequiredSkillIds.ToHashSet();
        var skills = (await _skillService.GetAllAsync())
            .Where(skill => skillIds.Contains(skill.SkillId)).ToList();

        return View(new JobDriveDetailsViewModel
        {
            CompanyId = result.Company!.CompanyId,
            CompanyName = result.Company.CompanyName,
            JobDrive = result.Job,
            RequiredSkills = skills
        });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int jobDriveId, int companyId)
    {
        var result = await GetOwnedJobAsync(jobDriveId, companyId);
        if (result.Error is not null) return result.Error;
        if (result.Job!.Status == JobDriveStatus.Closed)
        {
            TempData["ErrorMessage"] = "A closed JobDrive cannot be edited.";
            return RedirectToAction(nameof(Details), new { jobDriveId, companyId });
        }

        var model = new JobDriveFormViewModel
        {
            JobDriveId = result.Job.JobDriveId,
            CompanyId = result.Job.CompanyId,
            JobTitle = result.Job.JobTitle,
            JobDescription = result.Job.JobDescription,
            Location = result.Job.Location,
            MinCGPA = result.Job.MinCGPA,
            MaxBacklogs = result.Job.MaxBacklogs,
            GraduationYear = result.Job.GraduationYear,
            SalaryPackage = result.Job.SalaryPackage,
            ApplicationDeadline = result.Job.ApplicationDeadline.ToLocalTime(),
            RequiredSkillIds = result.Job.RequiredSkillIds,
            EligibleBranches = result.Job.EligibleBranches
        };
        await LoadFormChoicesAsync(model);
        return View("Create", model);
    }

    [HttpPost("/JobDrives/Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(JobDriveFormViewModel model)
    {
        if (model.JobDriveId is null || model.JobDriveId <= 0)
            return BadRequest("A valid JobDrive ID is required.");

        var ownership = await GetOwnedJobAsync(model.JobDriveId.Value, model.CompanyId);
        if (ownership.Error is not null) return ownership.Error;

        await ValidateFormAsync(model);
        if (!ModelState.IsValid)
        {
            await LoadFormChoicesAsync(model);
            return View("Create", model);
        }

        try
        {
            var updated = await _jobDriveService.UpdateAsync(model.JobDriveId.Value,
                ToUpdateDto(model));
            if (updated is null) return NotFound();

            TempData["SuccessMessage"] = $"JobDrive '{updated.JobTitle}' was updated.";
            return RedirectToAction(nameof(Details),
                new { jobDriveId = updated.JobDriveId, companyId = model.CompanyId });
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await LoadFormChoicesAsync(model);
            return View("Create", model);
        }
    }

    [HttpPost("/JobDrives/Publish")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Publish(int jobDriveId, int companyId)
    {
        var ownership = await GetOwnedJobAsync(jobDriveId, companyId);
        if (ownership.Error is not null) return ownership.Error;
        try
        {
            var job = await _jobDriveService.PublishAsync(jobDriveId);
            TempData["SuccessMessage"] = $"JobDrive '{job!.JobTitle}' was published.";
        }
        catch (InvalidOperationException exception)
        {
            TempData["ErrorMessage"] = exception.Message;
        }
        return RedirectToAction(nameof(Details), new { jobDriveId, companyId });
    }

    [HttpPost("/JobDrives/Close")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Close(int jobDriveId, int companyId)
    {
        var ownership = await GetOwnedJobAsync(jobDriveId, companyId);
        if (ownership.Error is not null) return ownership.Error;
        try
        {
            var job = await _jobDriveService.CloseAsync(jobDriveId);
            TempData["SuccessMessage"] = $"JobDrive '{job!.JobTitle}' was closed.";
        }
        catch (InvalidOperationException exception)
        {
            TempData["ErrorMessage"] = exception.Message;
        }
        return RedirectToAction(nameof(Details), new { jobDriveId, companyId });
    }

    private async Task ValidateFormAsync(JobDriveFormViewModel model)
    {
        if (model.CompanyId <= 0 || await _companyService.GetByIdAsync(model.CompanyId) is null)
            ModelState.AddModelError(nameof(model.CompanyId), "Company profile not found.");
        if (model.RequiredSkillIds.Count == 0)
            ModelState.AddModelError(nameof(model.RequiredSkillIds), "Select at least one required skill.");
        if (model.EligibleBranches.Count == 0)
            ModelState.AddModelError(nameof(model.EligibleBranches), "Select at least one eligible branch.");
        if (model.ApplicationDeadline <= DateTime.Now)
            ModelState.AddModelError(nameof(model.ApplicationDeadline), "Application deadline must be in the future.");
    }

    private async Task LoadFormChoicesAsync(JobDriveFormViewModel model) =>
        model.AvailableSkills = await _skillService.GetAllAsync();

    private async Task<(AIPlacement.Application.Company.DTOs.CompanyProfileDto? Value,
        IActionResult? Result)> GetCompanyAsync(int? companyId)
    {
        if (!int.TryParse(User.FindFirstValue("profile_id"), out var ownedCompanyId))
            return (null, Forbid());
        var company = await _companyService.GetByIdAsync(ownedCompanyId);
        return company is null
            ? (null, NotFound("Company profile not found."))
            : (company, null);
    }

    private async Task<(AIPlacement.Application.Company.DTOs.CompanyProfileDto? Company,
        JobDriveDto? Job, IActionResult? Error)> GetOwnedJobAsync(int jobDriveId, int companyId)
    {
        if (!int.TryParse(User.FindFirstValue("profile_id"), out companyId))
            return (null, null, Forbid());
        if (jobDriveId <= 0)
            return (null, null, BadRequest("Valid JobDrive and Company IDs are required."));
        var company = await _companyService.GetByIdAsync(companyId);
        if (company is null) return (null, null, NotFound("Company profile not found."));
        var job = await _jobDriveService.GetByIdAsync(jobDriveId);
        return job is null || job.CompanyId != companyId
            ? (company, null, NotFound("JobDrive not found for this company."))
            : (company, job, null);
    }

    private static CreateJobDriveDto ToCreateDto(JobDriveFormViewModel model) => new()
    {
        CompanyId = model.CompanyId,
        JobTitle = model.JobTitle,
        JobDescription = model.JobDescription,
        Location = model.Location,
        MinCGPA = model.MinCGPA,
        MaxBacklogs = model.MaxBacklogs,
        GraduationYear = model.GraduationYear,
        SalaryPackage = model.SalaryPackage,
        ApplicationDeadline = model.ApplicationDeadline.ToUniversalTime(),
        RequiredSkillIds = model.RequiredSkillIds,
        EligibleBranches = model.EligibleBranches
    };

    private static UpdateJobDriveDto ToUpdateDto(JobDriveFormViewModel model) => new()
    {
        JobTitle = model.JobTitle,
        JobDescription = model.JobDescription,
        Location = model.Location,
        MinCGPA = model.MinCGPA,
        MaxBacklogs = model.MaxBacklogs,
        GraduationYear = model.GraduationYear,
        SalaryPackage = model.SalaryPackage,
        ApplicationDeadline = model.ApplicationDeadline.ToUniversalTime(),
        RequiredSkillIds = model.RequiredSkillIds,
        EligibleBranches = model.EligibleBranches
    };

    private static bool IsValidStatus(string? status) => string.IsNullOrWhiteSpace(status) ||
        status.Equals(JobDriveStatus.Draft, StringComparison.OrdinalIgnoreCase) ||
        status.Equals(JobDriveStatus.Open, StringComparison.OrdinalIgnoreCase) ||
        status.Equals(JobDriveStatus.Closed, StringComparison.OrdinalIgnoreCase);

    private static bool IsValidApprovalStatus(string? status) => string.IsNullOrWhiteSpace(status) ||
        status.Equals(JobDriveApprovalStatus.Pending, StringComparison.OrdinalIgnoreCase) ||
        status.Equals(JobDriveApprovalStatus.Approved, StringComparison.OrdinalIgnoreCase) ||
        status.Equals(JobDriveApprovalStatus.Rejected, StringComparison.OrdinalIgnoreCase);
}
