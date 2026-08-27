using AIPlacement.Application.Company.Interfaces;
using AIPlacement.Application.Jobs;
using AIPlacement.Application.Jobs.Interfaces;
using AIPlacement.MVC.Models.CompanyAndJob;
using AIPlacement.Application.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AIPlacement.MVC.Controllers;

[Authorize(Roles = RoleNames.Company)]
public class CompanyDashboardController : Controller
{
    private readonly ICompanyService _companyService;
    private readonly IJobDriveService _jobDriveService;

    public CompanyDashboardController(
        ICompanyService companyService,
        IJobDriveService jobDriveService)
    {
        _companyService = companyService;
        _jobDriveService = jobDriveService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (!int.TryParse(User.FindFirstValue("profile_id"), out var companyId))
        {
            return Forbid();
        }

        var company = await _companyService.GetByIdAsync(companyId);

        if (company is null)
        {
            return NotFound("Company profile not found.");
        }

        var jobDrives =
            await _jobDriveService.GetByCompanyIdAsync(companyId);

        var model = new CompanyDashboardViewModel
        {
            CompanyId = company.CompanyId,
            CompanyName = company.CompanyName,
            TotalJobDrives = jobDrives.Count,
            DraftJobDrives = jobDrives.Count(job =>
                job.Status == JobDriveStatus.Draft),
            PendingApprovalJobDrives = jobDrives.Count(job =>
                job.ApprovalStatus == JobDriveApprovalStatus.Pending),
            OpenJobDrives = jobDrives.Count(job =>
                job.Status == JobDriveStatus.Open),
            ClosedJobDrives = jobDrives.Count(job =>
                job.Status == JobDriveStatus.Closed)
        };

        return View(model);
    }
}
