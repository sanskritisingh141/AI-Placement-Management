using AIPlacement.Application.Admin.DTOs;
using AIPlacement.Application.Admin.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.MVC.Controllers;

public class AdminController : Controller
{
    private const string SessionKeyAdminName = "AdminName";
    private const string SessionKeyAdminEmail = "AdminEmail";

    private readonly IAdminAuthService _adminAuthService;
    private readonly IUserRecordsService _userRecordsService;
    private readonly IJobDriveApprovalService _jobDriveApprovalService;
    private readonly IApplicationMonitoringService _applicationMonitoringService;
    private readonly IAnalyticsService _analyticsService;

    public AdminController(
        IAdminAuthService adminAuthService,
        IUserRecordsService userRecordsService,
        IJobDriveApprovalService jobDriveApprovalService,
        IApplicationMonitoringService applicationMonitoringService,
        IAnalyticsService analyticsService)
    {
        _adminAuthService = adminAuthService;
        _userRecordsService = userRecordsService;
        _jobDriveApprovalService = jobDriveApprovalService;
        _applicationMonitoringService = applicationMonitoringService;
        _analyticsService = analyticsService;
    }

    // GET: /Admin/Login
    [HttpGet]
    public IActionResult Login()
    {
        if (IsLoggedIn())
            return RedirectToAction(nameof(Dashboard));

        return View(new AdminLoginRequestDto());
    }

    // POST: /Admin/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AdminLoginRequestDto request)
    {
        var session = await _adminAuthService.LoginAsync(request);

        if (session == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(request);
        }

        HttpContext.Session.SetString(SessionKeyAdminName, session.Name);
        HttpContext.Session.SetString(SessionKeyAdminEmail, session.Email);

        return RedirectToAction(nameof(Dashboard));
    }

    // POST: /Admin/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }

    // GET: /Admin/Dashboard  (AD-US-05)
    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        if (!IsLoggedIn())
            return RedirectToAction(nameof(Login));

        ViewBag.AdminName = HttpContext.Session.GetString(SessionKeyAdminName);

        var summary = await _analyticsService.GetDashboardSummaryAsync();
        var branchStats = await _analyticsService.GetBranchPlacementStatsAsync();

        ViewBag.Summary = summary;
        return View(branchStats);
    }

    // GET: /Admin/Users  (AD-US-02)
    [HttpGet]
    public async Task<IActionResult> Users()
    {
        if (!IsLoggedIn())
            return RedirectToAction(nameof(Login));

        ViewBag.AdminName = HttpContext.Session.GetString(SessionKeyAdminName);
        ViewBag.Students = await _userRecordsService.GetStudentsAsync();
        ViewBag.Recruiters = await _userRecordsService.GetRecruitersAsync();

        return View();
    }

    // POST: /Admin/ToggleUserStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleUserStatus(int userId, bool isActive)
    {
        if (!IsLoggedIn())
            return RedirectToAction(nameof(Login));

        await _userRecordsService.SetActiveStatusAsync(userId, isActive);
        return RedirectToAction(nameof(Users));
    }

    // GET: /Admin/JobDriveApprovals  (AD-US-03)
    [HttpGet]
    public async Task<IActionResult> JobDriveApprovals()
    {
        if (!IsLoggedIn())
            return RedirectToAction(nameof(Login));

        ViewBag.AdminName = HttpContext.Session.GetString(SessionKeyAdminName);
        var drives = await _jobDriveApprovalService.GetAllAsync();

        return View(drives);
    }

    // POST: /Admin/ApproveJobDrive
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveJobDrive(int jobDriveId)
    {
        if (!IsLoggedIn())
            return RedirectToAction(nameof(Login));

        await _jobDriveApprovalService.ApproveAsync(jobDriveId);
        return RedirectToAction(nameof(JobDriveApprovals));
    }

    // POST: /Admin/RejectJobDrive
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectJobDrive(int jobDriveId, string reason)
    {
        if (!IsLoggedIn())
            return RedirectToAction(nameof(Login));

        await _jobDriveApprovalService.RejectAsync(jobDriveId, reason);
        return RedirectToAction(nameof(JobDriveApprovals));
    }

    // GET: /Admin/Applications  (AD-US-04)
    [HttpGet]
    public async Task<IActionResult> Applications()
    {
        if (!IsLoggedIn())
            return RedirectToAction(nameof(Login));

        ViewBag.AdminName = HttpContext.Session.GetString(SessionKeyAdminName);
        ViewBag.Applications = await _applicationMonitoringService.GetAllApplicationsAsync();
        ViewBag.Placements = await _applicationMonitoringService.GetPlacementResultsAsync();

        return View();
    }

    // GET: /Admin/ExportReport
    [HttpGet]
    public async Task<IActionResult> ExportReport()
    {
        if (!IsLoggedIn())
            return RedirectToAction(nameof(Login));

        var bytes = await _analyticsService.ExportPlacementReportCsvAsync();
        return File(bytes, "text/csv", "placement-report.csv");
    }

    private bool IsLoggedIn()
    {
        return !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyAdminEmail));
    }
}
