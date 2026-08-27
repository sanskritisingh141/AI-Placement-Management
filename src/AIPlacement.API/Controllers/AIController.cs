using System.Security.Claims;
using AIPlacement.Application.AI.Interfaces;
using AIPlacement.Application.Authentication;
using AIPlacement.Application.Resumes.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/ai")]
[Authorize]
public class AIController : ControllerBase
{
    private readonly IAIService _aiService;
    private readonly IResumeService _resumeService;
    private readonly IWebHostEnvironment _environment;

    public AIController(IAIService aiService, IResumeService resumeService, IWebHostEnvironment environment)
    {
        _aiService = aiService;
        _resumeService = resumeService;
        _environment = environment;
    }

    [Authorize(Roles = RoleNames.Student)]
    [HttpPost("resumes/{resumeId:int}/analyze")]
    public async Task<IActionResult> AnalyzeResume(int resumeId, CancellationToken cancellationToken)
    {
        var resume = await _resumeService.GetByIdAsync(resumeId);
        if (resume is null)
            return NotFound(new { message = "Resume not found." });
        if (!OwnsStudent(resume.StudentId))
            return Forbid();

        var fullPath = ResolvePrivateFile(resume.FilePath);
        if (fullPath is null || !System.IO.File.Exists(fullPath))
            return NotFound(new { message = "Resume file not found." });

        try
        {
            var bytes = await System.IO.File.ReadAllBytesAsync(fullPath, cancellationToken);
            return Ok(await _aiService.AnalyzeResumeAsync(resumeId, bytes, cancellationToken));
        }
        catch (HttpRequestException exception)
        {
            return StatusCode(503, new { message = "AI analysis service is unavailable.", detail = exception.Message });
        }
    }

    [Authorize(Roles = RoleNames.Student)]
    [HttpPost("job-drives/{jobDriveId:int}/match")]
    public async Task<IActionResult> CalculateMatch(int jobDriveId, CancellationToken cancellationToken)
    {
        if (!TryGetProfileId(out var studentId))
            return Forbid();

        try
        {
            return Ok(await _aiService.CalculateMatchAsync(studentId, jobDriveId, cancellationToken));
        }
        catch (ArgumentException exception)
        {
            return NotFound(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (HttpRequestException exception)
        {
            return StatusCode(503, new { message = "AI matching service is unavailable.", detail = exception.Message });
        }
    }

    [Authorize(Roles = RoleNames.Student)]
    [HttpGet("job-drives/{jobDriveId:int}/match")]
    public async Task<IActionResult> GetMatch(int jobDriveId)
    {
        if (!TryGetProfileId(out var studentId))
            return Forbid();

        var result = await _aiService.GetMatchAsync(studentId, jobDriveId);
        return result is null ? NotFound() : Ok(result);
    }

    private bool OwnsStudent(int studentId) =>
        TryGetProfileId(out var profileId) && profileId == studentId;

    private bool TryGetProfileId(out int profileId) =>
        int.TryParse(User.FindFirstValue("profile_id"), out profileId);

    private string? ResolvePrivateFile(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return null;

        var root = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "App_Data"));
        var fullPath = Path.GetFullPath(Path.Combine(
            root,
            relativePath.Replace('/', Path.DirectorySeparatorChar)));
        return fullPath.StartsWith(root + Path.DirectorySeparatorChar,
            StringComparison.OrdinalIgnoreCase) ? fullPath : null;
    }
}
