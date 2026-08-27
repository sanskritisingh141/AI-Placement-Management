using AIPlacement.Application.Jobs.DTOs;
using AIPlacement.Application.Jobs.Services;
using AIPlacement.Domain.Entities.Jobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AIPlacement.Application.Jobs.Interfaces;
using AIPlacement.Application.Recruitment.Interfaces;
using System.Security.Claims;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/job-eligible-branches")]
[Authorize(Roles = "Company,Admin")]
public class JobEligibleBranchController : ControllerBase
{
    private readonly IJobEligibleBranchService _service;
    private readonly IJobEligibleBranchRepository _repository;
    private readonly IRecruitmentRepository _recruitment;

    public JobEligibleBranchController(IJobEligibleBranchService service, IJobEligibleBranchRepository repository, IRecruitmentRepository recruitment)
        => (_service, _repository, _recruitment) = (service, repository, recruitment);

    [HttpGet("job-drive/{jobDriveId:int}")]
    public async Task<IActionResult> GetByJobDriveId(int jobDriveId)
    {
        if (!await OwnsJob(jobDriveId)) return Forbid();
        var branches = await _service.GetByJobDriveIdAsync(jobDriveId);
        return Ok(branches.Select(ToDto));
    }

    [HttpPost]
    public async Task<IActionResult> Add(JobEligibleBranchDto dto)
    {
        if (!await OwnsJob(dto.JobDriveId)) return Forbid();
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
        var existing = await _repository.GetByIdAsync(jobBranchId);
        if (existing is null) return NotFound();
        if (!await OwnsJob(existing.JobDriveId)) return Forbid();
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
    private async Task<bool> OwnsJob(int jobId) => User.IsInRole("Admin") ||
        await _recruitment.GetCompanyIdForJobDriveAsync(jobId) == int.Parse(User.FindFirstValue("profile_id")!);
}
