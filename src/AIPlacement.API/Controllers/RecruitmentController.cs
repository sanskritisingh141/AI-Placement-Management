using AIPlacement.Application.Recruitment.DTOs;
using AIPlacement.Application.Recruitment.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/recruitment")]
public class RecruitmentController : ControllerBase
{
    private readonly IRecruitmentService _recruitmentService;

    public RecruitmentController(IRecruitmentService recruitmentService)
    {
        _recruitmentService = recruitmentService;
    }

    [HttpGet("job-drives/{jobDriveId:int}/applicants")]
    public async Task<IActionResult> GetApplicants(int jobDriveId)
    {
        var applicants = await _recruitmentService.GetApplicantsAsync(jobDriveId);
        return Ok(applicants);
    }

    [HttpPatch("applications/{applicationId:int}/status")]
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
