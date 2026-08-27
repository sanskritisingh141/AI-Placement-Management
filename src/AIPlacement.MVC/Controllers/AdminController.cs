using AIPlacement.Application.Admin.DTOs;
using AIPlacement.Application.Admin.Interfaces;
using AIPlacement.Application.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.MVC.Controllers;

[Authorize(Roles = RoleNames.Admin)]
public class AdminController : Controller
{
    private readonly IUserRecordsService _userRecordsService;
    private readonly IJobDriveApprovalService _jobDriveApprovalService;
    private readonly IApplicationMonitoringService _applicationMonitoringService;
    private readonly IAnalyticsService _analyticsService;

    public AdminController(
        IUserRecordsService userRecordsService,
        IJobDriveApprovalService jobDriveApprovalService,
        IApplicationMonitoringService applicationMonitoringService,
        IAnalyticsService analyticsService)
    {
        _userRecordsService = userRecordsService;
        _jobDriveApprovalService = jobDriveApprovalService;
        _applicationMonitoringService = applicationMonitoringService;
        _analyticsService = analyticsService;
    }

    // GET: /Admin/Login
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        return RedirectToAction("Login", "Account");
    }

    // POST: /Admin/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }

    // GET: /Admin/Dashboard  (AD-US-05)
    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        ViewBag.AdminName = User.Identity?.Name;

        var summary = await _analyticsService.GetDashboardSummaryAsync();
        var branchStats = await _analyticsService.GetBranchPlacementStatsAsync();

        ViewBag.Summary = summary;
        return View(branchStats);
    }

    // GET: /Admin/Users  (AD-US-02)
    [HttpGet]
    public async Task<IActionResult> Users()
    {
        ViewBag.AdminName = User.Identity?.Name;
        ViewBag.Students = await _userRecordsService.GetStudentsAsync();
        ViewBag.Recruiters = await _userRecordsService.GetRecruitersAsync();

        return View();
    }

    // POST: /Admin/ToggleUserStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleUserStatus(int userId, bool isActive)
    {
        await _userRecordsService.SetActiveStatusAsync(userId, isActive);
        return RedirectToAction(nameof(Users));
    }

    // GET: /Admin/JobDriveApprovals  (AD-US-03)
    [HttpGet]
    public async Task<IActionResult> JobDriveApprovals()
    {
        ViewBag.AdminName = User.Identity?.Name;
        var drives = await _jobDriveApprovalService.GetAllAsync();

        return View(drives);
    }

    // POST: /Admin/ApproveJobDrive
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveJobDrive(int jobDriveId)
    {
        await _jobDriveApprovalService.ApproveAsync(jobDriveId);
        return RedirectToAction(nameof(JobDriveApprovals));
    }

    // POST: /Admin/RejectJobDrive
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectJobDrive(int jobDriveId, string reason)
    {
        await _jobDriveApprovalService.RejectAsync(jobDriveId, reason);
        return RedirectToAction(nameof(JobDriveApprovals));
    }

    // GET: /Admin/Applications  (AD-US-04)
    [HttpGet]
    public async Task<IActionResult> Applications()
    {
        ViewBag.AdminName = User.Identity?.Name;
        ViewBag.Applications = await _applicationMonitoringService.GetAllApplicationsAsync();
        ViewBag.Placements = await _applicationMonitoringService.GetPlacementResultsAsync();

        return View();
    }

    // GET: /Admin/ExportReport
    [HttpGet]
    public async Task<IActionResult> ExportReport()
    {
        var bytes = await _analyticsService.ExportPlacementReportCsvAsync();
        return File(bytes, "text/csv", "placement-report.csv");
    }

}
