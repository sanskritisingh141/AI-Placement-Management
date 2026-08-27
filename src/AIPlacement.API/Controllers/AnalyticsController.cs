using AIPlacement.Application.Admin.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AIPlacement.Application.Authentication;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/admin/analytics")]
[Authorize(Roles = RoleNames.Admin)]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await _analyticsService.GetDashboardSummaryAsync();
        return Ok(summary);
    }

    [HttpGet("branch-stats")]
    public async Task<IActionResult> GetBranchStats()
    {
        var stats = await _analyticsService.GetBranchPlacementStatsAsync();
        return Ok(stats);
    }

    [HttpGet("report/csv")]
    public async Task<IActionResult> ExportReport()
    {
        var bytes = await _analyticsService.ExportPlacementReportCsvAsync();
        return File(bytes, "text/csv", "placement-report.csv");
    }
}
