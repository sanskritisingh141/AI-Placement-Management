using System.Security.Claims;
using AIPlacement.Application.Authentication;
using AIPlacement.Application.Certifications.Interfaces;
using AIPlacement.Application.Projects.Interfaces;
using AIPlacement.Application.Recruitment.Interfaces;
using AIPlacement.Application.Resumes.Interfaces;
using AIPlacement.Application.Skills.Interfaces;
using AIPlacement.Application.Students.Interfaces;
using AIPlacement.MVC.Models.Student;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.MVC.Controllers;

[Authorize(Roles = RoleNames.Student)]
public class StudentDashboardController : Controller
{
    private readonly IStudentService _students;
    private readonly ISkillService _skills;
    private readonly IProjectService _projects;
    private readonly ICertificationService _certifications;
    private readonly IResumeService _resumes;
    private readonly IRecruitmentService _recruitment;

    public StudentDashboardController(
        IStudentService students,
        ISkillService skills,
        IProjectService projects,
        ICertificationService certifications,
        IResumeService resumes,
        IRecruitmentService recruitment)
    {
        _students = students;
        _skills = skills;
        _projects = projects;
        _certifications = certifications;
        _resumes = resumes;
        _recruitment = recruitment;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (!TryGetProfileId(out var studentId))
            return Forbid();

        var profile = await _students.GetByIdAsync(studentId);
        if (profile is null)
            return NotFound();

        var skills = (await _skills.GetByStudentIdAsync(studentId)).ToList();
        var projects = (await _projects.GetByStudentIdAsync(studentId)).ToList();
        var certifications = (await _certifications.GetByStudentIdAsync(studentId)).ToList();
        var resumes = (await _resumes.GetByStudentIdAsync(studentId)).ToList();
        var applications = await _recruitment.GetStudentApplicationsAsync(studentId);

        return View(new StudentDashboardViewModel
        {
            StudentName = User.Identity?.Name ?? "Student",
            Profile = profile,
            Skills = skills,
            Projects = projects,
            Certifications = certifications,
            Resumes = resumes,
            RecentApplications = applications.Take(5).ToList(),
            ProfileCompletion = CalculateCompletion(profile, skills.Count, projects.Count, resumes.Count)
        });
    }

    private bool TryGetProfileId(out int profileId) =>
        int.TryParse(User.FindFirstValue("profile_id"), out profileId);

    private static int CalculateCompletion(
        AIPlacement.Application.Students.DTOs.StudentDto profile,
        int skillCount,
        int projectCount,
        int resumeCount)
    {
        var completed = 0;
        completed += string.IsNullOrWhiteSpace(profile.RollNo) ? 0 : 1;
        completed += string.IsNullOrWhiteSpace(profile.Branch) ? 0 : 1;
        completed += profile.CGPA.HasValue ? 1 : 0;
        completed += profile.GraduationYear.HasValue ? 1 : 0;
        completed += skillCount > 0 ? 1 : 0;
        completed += projectCount > 0 ? 1 : 0;
        completed += resumeCount > 0 ? 1 : 0;
        return (int)Math.Round(completed / 7m * 100m);
    }
}
