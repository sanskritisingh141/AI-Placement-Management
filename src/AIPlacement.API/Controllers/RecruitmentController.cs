using AIPlacement.Application.Recruitment.DTOs;
using AIPlacement.Application.Recruitment.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/recruitment")]
[Authorize]
public class RecruitmentController : ControllerBase
{
    private readonly IRecruitmentService _recruitmentService;

    public RecruitmentController(IRecruitmentService recruitmentService)
    {
        _recruitmentService = recruitmentService;
    }

    [HttpPost("applications")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Apply(ApplyToJobDriveDto request)
    {
        try
        {
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
        var applicants = await _recruitmentService.GetApplicantsAsync(jobDriveId);
        return Ok(applicants);
    }

    [HttpPatch("applications/{applicationId:int}/status")]
    [Authorize(Roles = "Company,Admin")]
    public async Task<IActionResult> UpdateApplicationStatus(
        int applicationId,
        UpdateApplicationStatusDto request)
    {
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
}
