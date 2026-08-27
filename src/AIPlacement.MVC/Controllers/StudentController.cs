using AIPlacement.Application.Certifications.DTOs;
using AIPlacement.Application.Certifications.Interfaces;
using AIPlacement.Application.Projects.DTOs;
using AIPlacement.Application.Projects.Interfaces;
using AIPlacement.Application.Resumes.Interfaces;
using AIPlacement.Application.Skills.DTOs;
using AIPlacement.Application.Skills.Interfaces;
using AIPlacement.Application.Students.DTOs;
using AIPlacement.Application.Students.Interfaces;
using AIPlacement.Application.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace AIPlacement.MVC.Controllers;

[Authorize(Roles = RoleNames.Student)]
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

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!int.TryParse(User.FindFirstValue("profile_id"), out var studentId))
        {
            context.Result = Forbid();
            return;
        }

        foreach (var argumentName in context.ActionArguments.Keys.ToList())
        {
            if (string.Equals(argumentName, "studentId", StringComparison.OrdinalIgnoreCase))
                context.ActionArguments[argumentName] = studentId;

            switch (context.ActionArguments[argumentName])
            {
                case SkillDto skill:
                    skill.StudentId = studentId;
                    break;
                case ProjectDto project:
                    project.StudentId = studentId;
                    break;
                case CertificationDto certification:
                    certification.StudentId = studentId;
                    break;
            }
        }

        base.OnActionExecuting(context);
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
            return RedirectToAction(
                nameof(Skills),
                new { studentId = skill.StudentId });

        await _skillService.CreateAsync(skill);

        return RedirectToAction(
            nameof(Skills),
            new { studentId = skill.StudentId });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSkill(
        int skillId,
        SkillDto skill)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(
                nameof(Skills),
                new { studentId = skill.StudentId });

        var updated = await _skillService.UpdateAsync(
            skillId,
            skill);

        if (updated == null)
            return NotFound();

        return RedirectToAction(
            nameof(Skills),
            new { studentId = skill.StudentId });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSkill(
        int skillId,
        int studentId)
    {
        await _skillService.DeleteAsync(studentId, skillId);

        return RedirectToAction(
            nameof(Skills),
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


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddProject(ProjectDto project)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(
                nameof(Projects),
                new { studentId = project.StudentId });

        await _projectService.CreateAsync(project);

        return RedirectToAction(
            nameof(Projects),
            new { studentId = project.StudentId });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProject(
        int projectId,
        ProjectDto project)
    {
        var ownedProjects = await _projectService.GetByStudentIdAsync(project.StudentId);
        if (!ownedProjects.Any(item => item.ProjectId == projectId))
            return Forbid();

        if (!ModelState.IsValid)
            return RedirectToAction(
                nameof(Projects),
                new { studentId = project.StudentId });

        var updated = await _projectService.UpdateAsync(
            projectId,
            project);

        if (updated == null)
            return NotFound();

        return RedirectToAction(
            nameof(Projects),
            new { studentId = project.StudentId });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProject(
        int projectId,
        int studentId)
    {
        var ownedProjects = await _projectService.GetByStudentIdAsync(studentId);
        if (!ownedProjects.Any(item => item.ProjectId == projectId))
            return Forbid();

        await _projectService.DeleteAsync(projectId);

        return RedirectToAction(
            nameof(Projects),
            new { studentId });
    }


 
    [HttpGet]
    public async Task<IActionResult> Certifications(int studentId)
    {
        var certifications =
            await _certificationService.GetByStudentIdAsync(studentId);

        ViewBag.StudentId = studentId;

        return View(certifications);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCertification(
        CertificationDto certification)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(
                nameof(Certifications),
                new { studentId = certification.StudentId });

        await _certificationService.CreateAsync(certification);

        return RedirectToAction(
            nameof(Certifications),
            new { studentId = certification.StudentId });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCertification(
        int certificationId,
        CertificationDto certification)
    {
        var ownedCertifications = await _certificationService
            .GetByStudentIdAsync(certification.StudentId);
        if (!ownedCertifications.Any(item => item.CertificationId == certificationId))
            return Forbid();

        if (!ModelState.IsValid)
            return RedirectToAction(
                nameof(Certifications),
                new { studentId = certification.StudentId });

        var updated =
            await _certificationService.UpdateAsync(
                certificationId,
                certification);

        if (updated == null)
            return NotFound();

        return RedirectToAction(
            nameof(Certifications),
            new { studentId = certification.StudentId });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCertification(
        int certificationId,
        int studentId)
    {
        var ownedCertifications = await _certificationService.GetByStudentIdAsync(studentId);
        if (!ownedCertifications.Any(item => item.CertificationId == certificationId))
            return Forbid();

        await _certificationService.DeleteAsync(
            certificationId);

        return RedirectToAction(
            nameof(Certifications),
            new { studentId });
    }


  
    [HttpGet]
    public async Task<IActionResult> Resume(int studentId)
    {
        var resumes =
            await _resumeService.GetByStudentIdAsync(studentId);

        ViewBag.StudentId = studentId;

        return View(resumes);
    }
}
