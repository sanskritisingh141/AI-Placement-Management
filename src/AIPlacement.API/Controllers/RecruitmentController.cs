using AIPlacement.Application.Recruitment.DTOs;
using AIPlacement.Application.Recruitment.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AIPlacement.Application.Authentication;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/recruitment")]
[Authorize]
public class RecruitmentController : ControllerBase
{
    private readonly IRecruitmentService _recruitmentService;
    private readonly IRecruitmentRepository _repository;

    public RecruitmentController(IRecruitmentService recruitmentService, IRecruitmentRepository repository)
    {
        _recruitmentService = recruitmentService;
        _repository = repository;
    }

    [HttpPost("applications")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Apply(ApplyToJobDriveDto request)
    {
        try
        {
            request.StudentId = int.Parse(User.FindFirstValue("profile_id")!);
            var applicant = await _recruitmentService.ApplyAsync(request);
            return CreatedAtAction(
                nameof(GetApplicants),
                new { jobDriveId = applicant.JobDriveId },
                applicant);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpGet("job-drives/{jobDriveId:int}/applicants")]
    [Authorize(Roles = "Company,Admin")]
    public async Task<IActionResult> GetApplicants(int jobDriveId)
    {
        if (!await CanManageJobAsync(jobDriveId)) return Forbid();
        var applicants = await _recruitmentService.GetApplicantsAsync(jobDriveId);
        return Ok(applicants);
    }

    [HttpPatch("applications/{applicationId:int}/status")]
    [Authorize(Roles = "Company,Admin")]
    public async Task<IActionResult> UpdateApplicationStatus(
        int applicationId,
        UpdateApplicationStatusDto request)
    {
        if (!await CanManageApplicationAsync(applicationId)) return Forbid();
        request.ChangedByUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            var applicant = await _recruitmentService
                .UpdateApplicationStatusAsync(applicationId, request);

            return applicant is null
                ? NotFound(new { message = "Application not found." })
                : Ok(applicant);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("interview-rounds")]
    [Authorize(Roles = "Company,Admin")]
    public async Task<IActionResult> CreateInterviewRound(
        CreateInterviewRoundDto request)
    {
        if (!await CanManageJobAsync(request.JobDriveId)) return Forbid();
        try
        {
            var interviewRound = await _recruitmentService
                .CreateInterviewRoundAsync(request);

            return Ok(interviewRound);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("interviews")]
    [Authorize(Roles = "Company,Admin")]
    public async Task<IActionResult> ScheduleInterview(
        ScheduleInterviewDto request)
    {
        if (!await CanManageApplicationAsync(request.ApplicationId) || !await CanManageRoundAsync(request.RoundId)) return Forbid();
        try
        {
            var interview = await _recruitmentService
                .ScheduleInterviewAsync(request);

            return interview is null
                ? NotFound(new { message = "Application not found." })
                : Ok(interview);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("interviews/{interviewId:int}/result")]
    [Authorize(Roles = "Company,Admin")]
    public async Task<IActionResult> RecordInterviewResult(
        int interviewId,
        RecordInterviewResultDto request)
    {
        if (!await CanManageInterviewAsync(interviewId)) return Forbid();
        try
        {
            var interviewResult = await _recruitmentService
                .RecordInterviewResultAsync(interviewId, request);

            return interviewResult is null
                ? NotFound(new { message = "Interview schedule not found." })
                : Ok(interviewResult);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    private bool IsAdmin => User.IsInRole(RoleNames.Admin);
    private int CompanyId => int.Parse(User.FindFirstValue("profile_id")!);
    private async Task<bool> CanManageJobAsync(int id) => IsAdmin || await _repository.GetCompanyIdForJobDriveAsync(id) == CompanyId;
    private async Task<bool> CanManageApplicationAsync(int id) => IsAdmin || await _repository.GetCompanyIdForApplicationAsync(id) == CompanyId;
    private async Task<bool> CanManageRoundAsync(int id) => IsAdmin || await _repository.GetCompanyIdForRoundAsync(id) == CompanyId;
    private async Task<bool> CanManageInterviewAsync(int id) => IsAdmin || await _repository.GetCompanyIdForInterviewAsync(id) == CompanyId;
}
