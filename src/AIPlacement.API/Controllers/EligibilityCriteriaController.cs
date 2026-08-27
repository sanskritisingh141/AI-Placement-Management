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
[Route("api/eligibility-criteria")]
[Authorize(Roles = "Company,Admin")]
public class EligibilityCriteriaController : ControllerBase
{
    private readonly IEligibilityCriteriaService _service;
    private readonly IEligibilityCriteriaRepository _repository;
    private readonly IRecruitmentRepository _recruitment;

    public EligibilityCriteriaController(IEligibilityCriteriaService service, IEligibilityCriteriaRepository repository, IRecruitmentRepository recruitment)
        => (_service, _repository, _recruitment) = (service, repository, recruitment);

    [HttpGet("job-drive/{jobDriveId:int}")]
    public async Task<IActionResult> GetByJobDriveId(int jobDriveId)
    {
        if (!await OwnsJob(jobDriveId)) return Forbid();
        var criteria = await _service.GetByJobDriveIdAsync(jobDriveId);
        return criteria is null ? NotFound() : Ok(ToDto(criteria));
    }

    [HttpPost]
    public async Task<IActionResult> Add(EligibilityCriteriaDto dto)
    {
        if (!await OwnsJob(dto.JobDriveId)) return Forbid();
        try
        {
            var entity = ToEntity(dto);
            await _service.AddAsync(entity);
            return CreatedAtAction(nameof(GetByJobDriveId),
                new { jobDriveId = entity.JobDriveId }, ToDto(entity));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPut("{eligibilityId:int}")]
    public async Task<IActionResult> Update(int eligibilityId, EligibilityCriteriaDto dto)
    {
        var existing = await _repository.GetByIdAsync(eligibilityId);
        if (existing is null) return NotFound();
        if (!await OwnsJob(existing.JobDriveId) || dto.JobDriveId != existing.JobDriveId) return Forbid();
        if (dto.EligibilityId != 0 && dto.EligibilityId != eligibilityId)
            return BadRequest(new { message = "Eligibility ID does not match the route." });

        try
        {
            var entity = ToEntity(dto);
            entity.EligibilityId = eligibilityId;
            await _service.UpdateAsync(entity);
            return Ok(ToDto(entity));
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpDelete("{eligibilityId:int}")]
    public async Task<IActionResult> Delete(int eligibilityId)
    {
        var existing = await _repository.GetByIdAsync(eligibilityId);
        if (existing is null) return NotFound();
        if (!await OwnsJob(existing.JobDriveId)) return Forbid();
        try
        {
            await _service.DeleteAsync(eligibilityId);
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

    private static EligibilityCriteriaDto ToDto(EligibilityCriteria entity) => new()
    {
        EligibilityId = entity.EligibilityId,
        JobDriveId = entity.JobDriveId,
        MinCGPA = entity.MinCGPA,
        MaxBacklogs = entity.MaxBacklogs,
        GraduationYear = entity.GraduationYear
    };

    private static EligibilityCriteria ToEntity(EligibilityCriteriaDto dto) => new()
    {
        EligibilityId = dto.EligibilityId,
        JobDriveId = dto.JobDriveId,
        MinCGPA = dto.MinCGPA,
        MaxBacklogs = dto.MaxBacklogs,
        GraduationYear = dto.GraduationYear
    };
    private async Task<bool> OwnsJob(int jobId) => User.IsInRole("Admin") ||
        await _recruitment.GetCompanyIdForJobDriveAsync(jobId) == int.Parse(User.FindFirstValue("profile_id")!);
}
