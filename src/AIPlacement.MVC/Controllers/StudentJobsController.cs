using AIPlacement.Application.AI.Interfaces;
using AIPlacement.Application.Authentication;
using AIPlacement.Application.Jobs.Interfaces;
using AIPlacement.Application.Recruitment.DTOs;
using AIPlacement.Application.Recruitment.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AIPlacement.MVC.Controllers;

[Authorize(Roles = RoleNames.Student)]
public class StudentJobsController : Controller
{
    private readonly IJobDriveService _jobs;
    private readonly IRecruitmentService _recruitment;
    private readonly IAIService _ai;

    public StudentJobsController(IJobDriveService jobs, IRecruitmentService recruitment, IAIService ai)
        => (_jobs, _recruitment, _ai) = (jobs, recruitment, ai);

    public async Task<IActionResult> Index(string? search)
    {
        var jobs = await _jobs.GetAvailableAsync();
        if (!string.IsNullOrWhiteSpace(search))
            jobs = jobs.Where(x => x.JobTitle.Contains(search, StringComparison.OrdinalIgnoreCase)
                || x.Location.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
        return View(jobs);
    }

    public async Task<IActionResult> Details(int id)
    {
        var job = await _jobs.GetByIdAsync(id);
        if (job is null) return NotFound();
        ViewBag.Eligibility = await _recruitment.CheckEligibilityAsync(StudentId, id);
        ViewBag.Match = await _ai.GetMatchAsync(StudentId, id);
        return View(job);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CalculateMatch(int id)
    {
        try { await _ai.CalculateMatchAsync(StudentId, id); TempData["Success"] = "Match score calculated."; }
        catch (Exception ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(int id)
    {
        try
        {
            await _recruitment.ApplyAsync(new ApplyToJobDriveDto { StudentId = StudentId, JobDriveId = id });
            TempData["Success"] = "Application submitted.";
        }
        catch (Exception ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Details), new { id });
    }

    private int StudentId => int.Parse(User.FindFirstValue("profile_id")!);
}
