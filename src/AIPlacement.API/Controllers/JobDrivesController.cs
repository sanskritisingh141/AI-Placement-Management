using AIPlacement.Application.Jobs.DTOs;
using AIPlacement.Application.Jobs.Interfaces;
using AIPlacement.Application.Recruitment.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/job-drives")]
[Authorize]
public class JobDrivesController : ControllerBase
{
    private readonly IJobDriveService _jobDriveService;
    private readonly IRecruitmentService _recruitmentService;

    public JobDrivesController(
        IJobDriveService jobDriveService,
        IRecruitmentService recruitmentService)
    {
        _jobDriveService = jobDriveService;
        _recruitmentService = recruitmentService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAvailable()
    {
        var jobDrives = await _jobDriveService.GetAvailableAsync();
        return Ok(jobDrives);
    }

    [HttpGet("{jobDriveId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int jobDriveId)
    {
        var jobDrive = await _jobDriveService.GetByIdAsync(jobDriveId);

        return jobDrive is null
            ? NotFound(new { message = "Job drive not found." })
            : Ok(jobDrive);
    }

    [HttpGet("company/{companyId:int}")]
    [Authorize(Roles = "Company,Admin")]
    public async Task<IActionResult> GetByCompany(int companyId)
    {
        var jobDrives = await _jobDriveService.GetByCompanyIdAsync(companyId);
        return Ok(jobDrives);
    }

    [HttpGet("{jobDriveId:int}/check-eligibility/{studentId:int}")]
    [Authorize(Roles = "Student,Company,Admin")]
    public async Task<IActionResult> CheckEligibility(int jobDriveId, int studentId)
    {
        try
        {
            var result = await _recruitmentService
                .CheckEligibilityAsync(studentId, jobDriveId);
            return Ok(result);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> Create(CreateJobDriveDto request)
    {
        try
        {
            var jobDrive = await _jobDriveService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { jobDriveId = jobDrive.JobDriveId },
                jobDrive);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPut("{jobDriveId:int}")]
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> Update(
        int jobDriveId,
        UpdateJobDriveDto request)
    {
        try
        {
            var jobDrive = await _jobDriveService.UpdateAsync(jobDriveId, request);

            return jobDrive is null
                ? NotFound(new { message = "Job drive not found." })
                : Ok(jobDrive);
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

    [HttpPatch("{jobDriveId:int}/publish")]
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> Publish(int jobDriveId)
    {
        try
        {
            var jobDrive = await _jobDriveService.PublishAsync(jobDriveId);

            return jobDrive is null
                ? NotFound(new { message = "Job drive not found." })
                : Ok(jobDrive);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPatch("{jobDriveId:int}/close")]
    [Authorize(Roles = "Company")]
    public async Task<IActionResult> Close(int jobDriveId)
    {
        var jobDrive = await _jobDriveService.CloseAsync(jobDriveId);

        return jobDrive is null
            ? NotFound(new { message = "Job drive not found." })
            : Ok(jobDrive);
    }
}
