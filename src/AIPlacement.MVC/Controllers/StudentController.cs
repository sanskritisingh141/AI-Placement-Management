using AIPlacement.Application.Certifications.Interfaces;
using AIPlacement.Application.Projects.Interfaces;
using AIPlacement.Application.Resumes.Interfaces;
using AIPlacement.Application.Skills.DTOs;
using AIPlacement.Application.Skills.Interfaces;
using AIPlacement.Application.Students.DTOs;
using AIPlacement.Application.Students.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.MVC.Controllers;

public class StudentController : Controller
{
    private readonly IStudentService _studentService;
    private readonly ISkillService _skillService;
    private readonly IProjectService _projectService;
    private readonly ICertificationService _certificationService;
    private readonly IResumeService _resumeService;

    public StudentController(
        IStudentService studentService,
        ISkillService skillService,
        IProjectService projectService,
        ICertificationService certificationService,
        IResumeService resumeService)
    {
        _studentService = studentService;
        _skillService = skillService;
        _projectService = projectService;
        _certificationService = certificationService;
        _resumeService = resumeService;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard(int studentId)
    {
        var student = await _studentService.GetByIdAsync(studentId);

        if (student == null)
            return NotFound();

        ViewBag.Skills =
            await _skillService.GetByStudentIdAsync(studentId);

        ViewBag.Projects =
            await _projectService.GetByStudentIdAsync(studentId);

        ViewBag.Certifications =
            await _certificationService.GetByStudentIdAsync(studentId);

        ViewBag.Resumes =
            await _resumeService.GetByStudentIdAsync(studentId);

        return View(student);
    }

    [HttpGet]
    public async Task<IActionResult> Profile(int studentId)
    {
        var student = await _studentService.GetByIdAsync(studentId);

        if (student == null)
            return NotFound();

        return View(student);
    }

    [HttpGet]
    public async Task<IActionResult> Skills(int studentId)
    {
        var skills =
            await _skillService.GetByStudentIdAsync(studentId);

        ViewBag.StudentId = studentId;

        return View(skills);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSkill(SkillDto skill)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Skills),
                new { studentId = skill.StudentId });

        await _skillService.CreateAsync(skill);

        return RedirectToAction(nameof(Skills),
            new { studentId = skill.StudentId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSkill(
        int skillId,
        SkillDto skill)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Skills),
                new { studentId = skill.StudentId });

        var updated = await _skillService.UpdateAsync(
            skillId,
            skill);

        if (updated == null)
            return NotFound();

        return RedirectToAction(nameof(Skills),
            new { studentId = skill.StudentId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSkill(
        int skillId,
        int studentId)
    {
        await _skillService.DeleteAsync(skillId);

        return RedirectToAction(nameof(Skills),
            new { studentId });
    }

    [HttpGet]
    public async Task<IActionResult> Projects(int studentId)
    {
        var projects =
            await _projectService.GetByStudentIdAsync(studentId);

        ViewBag.StudentId = studentId;

        return View(projects);
    }

    [HttpGet]
    public async Task<IActionResult> Certifications(int studentId)
    {
        var certifications =
            await _certificationService.GetByStudentIdAsync(studentId);

        ViewBag.StudentId = studentId;

        return View(certifications);
    }

    [HttpGet]
    public async Task<IActionResult> Resume(int studentId)
    {
        var resumes =
            await _resumeService.GetByStudentIdAsync(studentId);

        ViewBag.StudentId = studentId;

        return View(resumes);
    }

    [HttpGet]
    public async Task<IActionResult> EditProfile(int studentId)
    {
        var student = await _studentService.GetByIdAsync(studentId);

        if (student == null)
            return NotFound();

        return View(student);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(
    int studentId,
    StudentDto student)
    {
        if (!ModelState.IsValid)
            return View(student);

        var updatedStudent = await _studentService.UpdateAsync(
            studentId,
            student);

        if (updatedStudent == null)
            return NotFound();

        return RedirectToAction(
            nameof(Profile),
            new { studentId });
    }
}
