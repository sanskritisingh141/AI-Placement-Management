using AIPlacement.Application.Company.Interfaces;
using AIPlacement.Application.Jobs;
using AIPlacement.Application.Jobs.DTOs;
using AIPlacement.Application.Jobs.Interfaces;
using AIPlacement.Application.Skills.Interfaces;
using AIPlacement.MVC.Models.CompanyAndJob;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.MVC.Controllers;

public class JobDrivesController : Controller
{
    private readonly ICompanyService _companyService;
    private readonly IJobDriveService _jobDriveService;
    private readonly ISkillService _skillService;

    public JobDrivesController(
        ICompanyService companyService,
        IJobDriveService jobDriveService,
        ISkillService skillService)
    {
        _companyService = companyService;
        _jobDriveService = jobDriveService;
        _skillService = skillService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        int? companyId,
        string? status,
        string? approvalStatus)
    {
        // TODO: Replace companyId with the authenticated company's ID.
        if (companyId is null || companyId <= 0)
        {
            return BadRequest("A valid company ID is required.");
        }

        if (!IsValidStatus(status))
        {
            return BadRequest("Invalid JobDrive status filter.");
        }

        if (!IsValidApprovalStatus(approvalStatus))
        {
            return BadRequest("Invalid approval status filter.");
        }

        var company =
            await _companyService.GetByIdAsync(companyId.Value);

        if (company is null)
        {
            return NotFound("Company profile not found.");
        }

        var jobDrives =
            await _jobDriveService.GetByCompanyIdAsync(
                companyId.Value);

        var filteredJobDrives = jobDrives
            .Where(job =>
                string.IsNullOrWhiteSpace(status) ||
                job.Status.Equals(
                    status,
                    StringComparison.OrdinalIgnoreCase))
            .Where(job =>
                string.IsNullOrWhiteSpace(approvalStatus) ||
                job.ApprovalStatus.Equals(
                    approvalStatus,
                    StringComparison.OrdinalIgnoreCase))
            .ToList();

        var model = new JobDriveListViewModel
        {
            CompanyId = company.CompanyId,
            CompanyName = company.CompanyName,
            SelectedStatus = status,
            SelectedApprovalStatus = approvalStatus,
            JobDrives = filteredJobDrives
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? companyId)
    {
        // TODO: Replace companyId with the authenticated company's ID.
        if (companyId is null || companyId <= 0)
        {
            return BadRequest("A valid company ID is required.");
        }

        var company =
            await _companyService.GetByIdAsync(companyId.Value);

        if (company is null)
        {
            return NotFound("Company profile not found.");
        }

        var model = new JobDriveFormViewModel
        {
            CompanyId = company.CompanyId,
            GraduationYear = DateTime.UtcNow.Year,
            ApplicationDeadline = DateTime.Now.AddDays(7),
            AvailableSkills = await _skillService.GetAllAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        JobDriveFormViewModel model)
    {
        // TODO: Verify CompanyId against the authenticated company.
        var company = model.CompanyId > 0
            ? await _companyService.GetByIdAsync(model.CompanyId)
            : null;

        if (company is null)
        {
            ModelState.AddModelError(
                nameof(model.CompanyId),
                "Company profile not found.");
        }

        if (model.RequiredSkillIds.Count == 0)
        {
            ModelState.AddModelError(
                nameof(model.RequiredSkillIds),
                "Select at least one required skill.");
        }

        if (model.EligibleBranches.Count == 0)
        {
            ModelState.AddModelError(
                nameof(model.EligibleBranches),
                "Select at least one eligible branch.");
        }

        if (model.ApplicationDeadline <= DateTime.Now)
        {
            ModelState.AddModelError(
                nameof(model.ApplicationDeadline),
                "Application deadline must be in the future.");
        }

        if (!ModelState.IsValid)
        {
            await LoadFormChoicesAsync(model);
            return View(model);
        }

        var request = new CreateJobDriveDto
        {
            CompanyId = model.CompanyId,
            JobTitle = model.JobTitle,
            JobDescription = model.JobDescription,
            Location = model.Location,
            MinCGPA = model.MinCGPA,
            MaxBacklogs = model.MaxBacklogs,
            GraduationYear = model.GraduationYear,
            SalaryPackage = model.SalaryPackage,
            ApplicationDeadline =
                model.ApplicationDeadline.ToUniversalTime(),
            RequiredSkillIds = model.RequiredSkillIds,
            EligibleBranches = model.EligibleBranches
        };

        try
        {
            var created =
                await _jobDriveService.CreateAsync(request);

            TempData["SuccessMessage"] =
                $"JobDrive '{created.JobTitle}' was created.";

            return RedirectToAction(
                nameof(Index),
                new { companyId = model.CompanyId });
        }
        catch (Exception exception) when (
            exception is ArgumentException ||
            exception is InvalidOperationException)
        {
            ModelState.AddModelError(
                string.Empty,
                exception.Message);

            await LoadFormChoicesAsync(model);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(
        int jobDriveId,
        int companyId)
    {
        if (jobDriveId <= 0 || companyId <= 0)
        {
            return BadRequest(
                "Valid JobDrive and Company IDs are required.");
        }

        var company =
            await _companyService.GetByIdAsync(companyId);

        if (company is null)
        {
            return NotFound("Company profile not found.");
        }

        var jobDrive =
            await _jobDriveService.GetByIdAsync(jobDriveId);

        if (jobDrive is null ||
            jobDrive.CompanyId != companyId)
        {
            return NotFound(
                "JobDrive not found for this company.");
        }

        var requiredSkillIds =
            jobDrive.RequiredSkillIds.ToHashSet();

        var requiredSkills =
            (await _skillService.GetAllAsync())
            .Where(skill =>
                requiredSkillIds.Contains(skill.SkillId))
            .ToList();

        var model = new JobDriveDetailsViewModel
        {
            CompanyId = company.CompanyId,
            CompanyName = company.CompanyName,
            JobDrive = jobDrive,
            RequiredSkills = requiredSkills
        };

        return View(model);
    }

    private async Task LoadFormChoicesAsync(
        JobDriveFormViewModel model)
    {
        model.AvailableSkills =
            await _skillService.GetAllAsync();
    }

    private static bool IsValidStatus(string? status)
    {
        return string.IsNullOrWhiteSpace(status) ||
               status.Equals(
                   JobDriveStatus.Draft,
                   StringComparison.OrdinalIgnoreCase) ||
               status.Equals(
                   JobDriveStatus.Open,
                   StringComparison.OrdinalIgnoreCase) ||
               status.Equals(
                   JobDriveStatus.Closed,
                   StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsValidApprovalStatus(
        string? approvalStatus)
    {
        return string.IsNullOrWhiteSpace(approvalStatus) ||
               approvalStatus.Equals(
                   JobDriveApprovalStatus.Pending,
                   StringComparison.OrdinalIgnoreCase) ||
               approvalStatus.Equals(
                   JobDriveApprovalStatus.Approved,
                   StringComparison.OrdinalIgnoreCase) ||
               approvalStatus.Equals(
                   JobDriveApprovalStatus.Rejected,
                   StringComparison.OrdinalIgnoreCase);
    }
}