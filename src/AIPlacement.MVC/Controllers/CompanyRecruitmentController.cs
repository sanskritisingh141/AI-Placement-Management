using AIPlacement.Application.Authentication;
using AIPlacement.Application.Jobs.Interfaces;
using AIPlacement.Application.Recruitment.DTOs;
using AIPlacement.Application.Recruitment.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AIPlacement.Application.Resumes.Interfaces;

namespace AIPlacement.MVC.Controllers;

[Authorize(Roles = RoleNames.Company)]
public class CompanyRecruitmentController(
    IJobDriveService jobs,
    IRecruitmentService recruitment,
    IRecruitmentRepository repository,
    IResumeService resumes,
    IWebHostEnvironment environment) : Controller
{
    private int CompanyId => int.Parse(User.FindFirstValue("profile_id")!);
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index()
    {
        var companyJobs = await jobs.GetByCompanyIdAsync(CompanyId);
        return View(companyJobs);
    }

    public async Task<IActionResult> Applicants(int jobDriveId)
    {
        if (!await OwnsJob(jobDriveId)) return Forbid();
        ViewBag.Job = await jobs.GetByIdAsync(jobDriveId);
        ViewBag.Rounds = await recruitment.GetInterviewRoundsAsync(jobDriveId);
        ViewBag.Schedules = await recruitment.GetInterviewSchedulesAsync(jobDriveId);
        return View(await recruitment.GetApplicantsAsync(jobDriveId));
    }

    [HttpGet]
    public async Task<IActionResult> ApplicantResume(int applicationId)
    {
        if (!await OwnsApplication(applicationId)) return Forbid();
        var application = await repository.GetApplicationByIdAsync(applicationId);
        if (application is null) return NotFound();
        var current = (await resumes.GetByStudentIdAsync(application.StudentId))
            .Where(item => item.IsCurrent == true).OrderByDescending(item => item.VersionNo).FirstOrDefault();
        if (current is null || string.IsNullOrWhiteSpace(current.FilePath)) return NotFound("No current resume.");
        var root = Path.GetFullPath(Path.Combine(environment.ContentRootPath, "App_Data"));
        var path = Path.GetFullPath(Path.Combine(root,
            current.FilePath.Replace('/', Path.DirectorySeparatorChar)));
        if (!path.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
            !System.IO.File.Exists(path)) return NotFound();
        return PhysicalFile(path, "application/pdf", current.FileName ?? "resume.pdf");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int applicationId, int jobDriveId, string status, string? remarks)
    {
        if (!await OwnsApplication(applicationId) || !await OwnsJob(jobDriveId)) return Forbid();
        try
        {
            await recruitment.UpdateApplicationStatusAsync(applicationId, new UpdateApplicationStatusDto
                { Status = status, Remarks = remarks, ChangedByUserId = UserId });
            TempData["Success"] = "Application status updated.";
        }
        catch (Exception ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Applicants), new { jobDriveId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddRound(CreateInterviewRoundDto model)
    {
        if (!await OwnsJob(model.JobDriveId)) return Forbid();
        await recruitment.CreateInterviewRoundAsync(model);
        TempData["Success"] = "Interview round created.";
        return RedirectToAction(nameof(Applicants), new { jobDriveId = model.JobDriveId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Schedule(int jobDriveId, ScheduleInterviewDto model)
    {
        if (!await OwnsJob(jobDriveId) || !await OwnsApplication(model.ApplicationId)
            || await repository.GetCompanyIdForRoundAsync(model.RoundId) != CompanyId) return Forbid();
        await recruitment.ScheduleInterviewAsync(model);
        TempData["Success"] = "Interview scheduled.";
        return RedirectToAction(nameof(Applicants), new { jobDriveId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RecordResult(int jobDriveId, int interviewId, RecordInterviewResultDto model)
    {
        if (!await OwnsJob(jobDriveId) || await repository.GetCompanyIdForInterviewAsync(interviewId) != CompanyId) return Forbid();
        await recruitment.RecordInterviewResultAsync(interviewId, model);
        TempData["Success"] = "Interview result recorded.";
        return RedirectToAction(nameof(Applicants), new { jobDriveId });
    }

    private async Task<bool> OwnsJob(int id) => await repository.GetCompanyIdForJobDriveAsync(id) == CompanyId;
    private async Task<bool> OwnsApplication(int id) => await repository.GetCompanyIdForApplicationAsync(id) == CompanyId;
}
