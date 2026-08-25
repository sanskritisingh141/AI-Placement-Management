using AIPlacement.Application.Jobs.DTOs;
using AIPlacement.Application.Jobs.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/job-drives")]
public class JobDrivesController : ControllerBase
{
    private readonly IJobDriveService _jobDriveService;

    public JobDrivesController(IJobDriveService jobDriveService)
    {
        _jobDriveService = jobDriveService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAvailable()
    {
        var jobDrives = await _jobDriveService.GetAvailableAsync();
        return Ok(jobDrives);
    }

    [HttpGet("{jobDriveId:int}")]
    public async Task<IActionResult> GetById(int jobDriveId)
    {
        var jobDrive = await _jobDriveService.GetByIdAsync(jobDriveId);

        return jobDrive is null
            ? NotFound(new { message = "Job drive not found." })
            : Ok(jobDrive);
    }

    [HttpGet("company/{companyId:int}")]
    public async Task<IActionResult> GetByCompany(int companyId)
    {
        var jobDrives = await _jobDriveService.GetByCompanyIdAsync(companyId);
        return Ok(jobDrives);
    }

    [HttpPost]
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
    public async Task<IActionResult> Close(int jobDriveId)
    {
        var jobDrive = await _jobDriveService.CloseAsync(jobDriveId);

        return jobDrive is null
            ? NotFound(new { message = "Job drive not found." })
            : Ok(jobDrive);
    }
}
