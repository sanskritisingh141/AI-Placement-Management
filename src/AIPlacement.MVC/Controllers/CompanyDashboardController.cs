using AIPlacement.Application.Company.Interfaces;
using AIPlacement.Application.Jobs;
using AIPlacement.Application.Jobs.Interfaces;
using AIPlacement.MVC.Models.CompanyAndJob;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.MVC.Controllers;

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
    public async Task<IActionResult> Index(int? companyId)
    {
        // TODO: Replace companyId with the authenticated company's ID
        // when shared role-based authentication is integrated.
        if (companyId is null || companyId <= 0)
        {
            return BadRequest("A valid company ID is required.");
        }

        var company = await _companyService.GetByIdAsync(companyId.Value);

        if (company is null)
        {
            return NotFound("Company profile not found.");
        }

        var jobDrives =
            await _jobDriveService.GetByCompanyIdAsync(companyId.Value);

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