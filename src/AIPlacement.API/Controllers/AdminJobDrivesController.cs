using AIPlacement.Application.Admin.Interfaces;
using AIPlacement.Application.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/admin/job-drives")]
[Authorize(Roles = RoleNames.Admin)]
public class AdminJobDrivesController : ControllerBase
{
    private readonly IJobDriveApprovalService _approvalService;

    public AdminJobDrivesController(IJobDriveApprovalService approvalService)
    {
        _approvalService = approvalService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _approvalService.GetAllAsync());

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending() =>
        Ok(await _approvalService.GetPendingAsync());

    [HttpPatch("{jobDriveId:int}/approve")]
    public async Task<IActionResult> Approve(int jobDriveId)
    {
        var jobDrive = await _approvalService.ApproveAsync(jobDriveId);
        return jobDrive is null
            ? NotFound(new { message = "Job drive not found." })
            : Ok(jobDrive);
    }

    [HttpPatch("{jobDriveId:int}/reject")]
    public async Task<IActionResult> Reject(
        int jobDriveId,
        [FromBody] RejectJobDriveRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
            return BadRequest(new { message = "A rejection reason is required." });

        var jobDrive = await _approvalService.RejectAsync(
            jobDriveId,
            request.Reason.Trim());

        return jobDrive is null
            ? NotFound(new { message = "Job drive not found." })
            : Ok(jobDrive);
    }
}

public sealed class RejectJobDriveRequest
{
    public string Reason { get; set; } = string.Empty;
}
