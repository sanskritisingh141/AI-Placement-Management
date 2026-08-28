using System.Security.Claims;
using AIPlacement.Application.AI.DTOs;
using AIPlacement.Application.AI.Interfaces;
using AIPlacement.Application.Authentication;
using AIPlacement.Application.Resumes.Interfaces;
using AIPlacement.MVC.Models.StudentResumes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.MVC.Controllers;

[Authorize(Roles = RoleNames.Student)]
public class StudentResumesController(
    IResumeService resumes,
    IAIService ai,
    IWebHostEnvironment environment) : Controller
{
    private const long MaximumBytes = 5 * 1024 * 1024;
    private int StudentId => int.Parse(User.FindFirstValue("profile_id")!);

    public async Task<IActionResult> Index()
    {
        var studentResumes = (await resumes.GetByStudentIdAsync(StudentId)).ToList();
        var latestAnalyses = new Dictionary<int, ResumeAnalysisResultDto>();
        foreach (var resume in studentResumes)
        {
            var analysis = await ai.GetLatestAnalysisAsync(resume.ResumeId);
            if (analysis is not null)
                latestAnalyses[resume.ResumeId] = analysis;
        }

        return View(new StudentResumeIndexViewModel
        {
            Resumes = studentResumes,
            LatestAnalyses = latestAnalyses
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file is null || file.Length is <= 0 or > MaximumBytes)
        {
            TempData["Error"] = "Select a PDF up to 5 MB.";
            return RedirectToAction(nameof(Index));
        }

        await using var input = file.OpenReadStream();
        var signature = new byte[5];
        if (!string.Equals(Path.GetExtension(file.FileName), ".pdf", StringComparison.OrdinalIgnoreCase) ||
            await input.ReadAsync(signature) != signature.Length ||
            !signature.SequenceEqual("%PDF-"u8.ToArray()))
        {
            TempData["Error"] = "Only valid PDF files are accepted.";
            return RedirectToAction(nameof(Index));
        }

        input.Position = 0;
        var relativePath = Path.Combine("resumes", StudentId.ToString(), $"{Guid.NewGuid():N}.pdf");
        var fullPath = Path.Combine(environment.ContentRootPath, "App_Data", relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        try
        {
            await using (var output = System.IO.File.Create(fullPath))
                await input.CopyToAsync(output);
            await resumes.AddVersionAsync(StudentId, Path.GetFileName(file.FileName), relativePath.Replace('\\', '/'));
            TempData["Success"] = "Resume uploaded as a new version.";
        }
        catch
        {
            if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Download(int id)
    {
        var resume = await resumes.GetByIdAsync(id);
        var fullPath = ResolvePrivateFile(resume?.FilePath);
        if (resume?.StudentId != StudentId || fullPath is null || !System.IO.File.Exists(fullPath))
            return NotFound();
        return PhysicalFile(fullPath, "application/pdf", resume.FileName ?? "resume.pdf");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Analyze(int id)
    {
        var resume = await resumes.GetByIdAsync(id);
        var fullPath = ResolvePrivateFile(resume?.FilePath);
        if (resume?.StudentId != StudentId || fullPath is null || !System.IO.File.Exists(fullPath))
            return NotFound();
        try
        {
            await ai.AnalyzeResumeAsync(id, await System.IO.File.ReadAllBytesAsync(fullPath));
            TempData["Success"] = "Resume analysis completed.";
        }
        catch (Exception exception)
        {
            TempData["Error"] = exception.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    private string? ResolvePrivateFile(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return null;
        var root = Path.GetFullPath(Path.Combine(environment.ContentRootPath, "App_Data"));
        var fullPath = Path.GetFullPath(Path.Combine(root,
            relativePath.Replace('/', Path.DirectorySeparatorChar)));
        return fullPath.StartsWith(root + Path.DirectorySeparatorChar,
            StringComparison.OrdinalIgnoreCase) ? fullPath : null;
    }
}
