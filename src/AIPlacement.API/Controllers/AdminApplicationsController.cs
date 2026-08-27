using AIPlacement.Application.Admin.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AIPlacement.Application.Authentication;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/admin/applications")]
[Authorize(Roles = RoleNames.Admin)]
public class AdminApplicationsController : ControllerBase
{
    private readonly IApplicationMonitoringService _applicationMonitoringService;

    public AdminApplicationsController(IApplicationMonitoringService applicationMonitoringService)
    {
        _applicationMonitoringService = applicationMonitoringService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status)
    {
        var applications = string.IsNullOrWhiteSpace(status)
            ? await _applicationMonitoringService.GetAllApplicationsAsync()
            : await _applicationMonitoringService.GetApplicationsByStatusAsync(status);

        return Ok(applications);
    }

    [HttpGet("placements")]
    public async Task<IActionResult> GetPlacements()
    {
        var placements = await _applicationMonitoringService.GetPlacementResultsAsync();
        return Ok(placements);
    }
}
