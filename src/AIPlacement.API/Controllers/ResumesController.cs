using AIPlacement.Application.Resumes.DTOs;
using AIPlacement.Application.Resumes.Interfaces;
using AIPlacement.Application.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AIPlacement.Application.Recruitment.Interfaces;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResumesController : ControllerBase
{
    private readonly IResumeService _resumeService;
    private readonly IWebHostEnvironment _environment;
    private readonly IRecruitmentRepository _recruitment;

    public ResumesController(
        IResumeService resumeService,
        IWebHostEnvironment environment,
        IRecruitmentRepository recruitment)
    {
        _resumeService = resumeService;
        _environment = environment;
        _recruitment = recruitment;
    }

    [HttpGet("{resumeId:int}")]
    public async Task<IActionResult> GetById(int resumeId)
    {
        var resume = await _resumeService.GetByIdAsync(resumeId);

        if (resume == null)
            return NotFound();
        if (!await CanAccessStudentAsync(resume.StudentId)) return Forbid();

        return Ok(resume);
    }

    [HttpGet("student/{studentId:int}")]
    public async Task<IActionResult> GetByStudentId(int studentId)
    {
        if (!await CanAccessStudentAsync(studentId)) return Forbid();
        var resumes = await _resumeService.GetByStudentIdAsync(studentId);

        return Ok(resumes);
    }

    [Authorize(Roles = RoleNames.Student)]
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> Upload([FromForm] ResumeUploadRequest request)
    {
        if (!TryGetProfileId(out var studentId))
            return Forbid();

        var file = request.File;

        if (file.Length is <= 0 or > 5 * 1024 * 1024)
            return BadRequest(new { message = "PDF files must be between 1 byte and 5 MB." });

        if (!string.Equals(Path.GetExtension(file.FileName), ".pdf", StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(file.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "Only PDF resume files are accepted." });
        }

        await using var input = file.OpenReadStream();
        var signature = new byte[5];
        if (await input.ReadAsync(signature) != signature.Length ||
            !signature.SequenceEqual("%PDF-"u8.ToArray()))
        {
            return BadRequest(new { message = "The uploaded file is not a valid PDF." });
        }

        input.Position = 0;
        var relativePath = Path.Combine(
            "resumes",
            studentId.ToString(),
            $"{Guid.NewGuid():N}.pdf");
        var fullPath = Path.Combine(_environment.ContentRootPath, "App_Data", relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        try
        {
            await using (var output = System.IO.File.Create(fullPath))
                await input.CopyToAsync(output);

            var resume = await _resumeService.AddVersionAsync(
                studentId,
                Path.GetFileName(file.FileName),
                relativePath.Replace('\\', '/'));

            return Ok(resume);
        }
        catch
        {
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
            throw;
        }
    }

    [Authorize]
    [HttpGet("{resumeId:int}/download")]
    public async Task<IActionResult> Download(int resumeId)
    {
        var resume = await _resumeService.GetByIdAsync(resumeId);
        if (resume is null)
            return NotFound();

        if (!await CanAccessStudentAsync(resume.StudentId)) return Forbid();

        var fullPath = ResolvePrivateFile(resume.FilePath);
        if (fullPath is null || !System.IO.File.Exists(fullPath))
            return NotFound();

        return PhysicalFile(fullPath, "application/pdf", resume.FileName ?? "resume.pdf");
    }

    [Authorize(Roles = RoleNames.Student)]
    [HttpDelete("{resumeId:int}")]
    public async Task<IActionResult> Delete(int resumeId)
    {
        var resume = await _resumeService.GetByIdAsync(resumeId);
        if (resume is null) return NotFound();
        if (!TryGetProfileId(out var studentId) || resume.StudentId != studentId) return Forbid();
        var path = ResolvePrivateFile(resume.FilePath);
        var deleted = await _resumeService.DeleteAsync(resumeId);

        if (!deleted)
            return NotFound();

        if (path is not null && System.IO.File.Exists(path)) System.IO.File.Delete(path);
        return NoContent();
    }

    private bool TryGetProfileId(out int profileId) =>
        int.TryParse(User.FindFirstValue("profile_id"), out profileId);

    private async Task<bool> CanAccessStudentAsync(int studentId)
    {
        if (User.IsInRole(RoleNames.Admin)) return true;
        if (User.IsInRole(RoleNames.Student))
            return TryGetProfileId(out var ownedStudentId) && ownedStudentId == studentId;
        return User.IsInRole(RoleNames.Company) && TryGetProfileId(out var companyId) &&
            await _recruitment.CompanyHasApplicantAsync(companyId, studentId);
    }

    private string? ResolvePrivateFile(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return null;
        var root = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "App_Data"));
        var fullPath = Path.GetFullPath(Path.Combine(root,
            relativePath.Replace('/', Path.DirectorySeparatorChar)));
        return fullPath.StartsWith(root + Path.DirectorySeparatorChar,
            StringComparison.OrdinalIgnoreCase) ? fullPath : null;
    }
}

public sealed class ResumeUploadRequest
{
    public IFormFile File { get; set; } = null!;
}
