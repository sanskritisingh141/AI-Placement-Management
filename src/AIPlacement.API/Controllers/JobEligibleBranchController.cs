using AIPlacement.Application.Jobs.DTOs;
using AIPlacement.Application.Jobs.Services;
using AIPlacement.Domain.Entities.Jobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/job-eligible-branches")]
[Authorize(Roles = "Company,Admin")]
public class JobEligibleBranchController : ControllerBase
{
    private readonly IJobEligibleBranchService _service;

    public JobEligibleBranchController(IJobEligibleBranchService service) => _service = service;

    [HttpGet("job-drive/{jobDriveId:int}")]
    public async Task<IActionResult> GetByJobDriveId(int jobDriveId)
    {
        var branches = await _service.GetByJobDriveIdAsync(jobDriveId);
        return Ok(branches.Select(ToDto));
    }

    [HttpPost]
    public async Task<IActionResult> Add(JobEligibleBranchDto dto)
    {
        try
        {
            var entity = new JobEligibleBranch
            {
                JobDriveId = dto.JobDriveId,
                BranchName = dto.BranchName
            };
            await _service.AddAsync(entity);
            return Ok(ToDto(entity));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpDelete("{jobBranchId:int}")]
    public async Task<IActionResult> Delete(int jobBranchId)
    {
        try
        {
            await _service.DeleteAsync(jobBranchId);
            return NoContent();
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(new { message = exception.Message });
        }
    }

    private static JobEligibleBranchDto ToDto(JobEligibleBranch entity) => new()
    {
        JobBranchId = entity.JobBranchId,
        JobDriveId = entity.JobDriveId,
        BranchName = entity.BranchName
    };
}
