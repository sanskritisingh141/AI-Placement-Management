using AIPlacement.Application.Projects.DTOs;
using AIPlacement.Application.Projects.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AIPlacement.Application.Authentication;
using System.Security.Claims;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = RoleNames.Student)]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet("{projectId}")]
    public async Task<ActionResult<ProjectDto>> GetById(int projectId)
    {
        var project =
            await _projectService.GetByIdAsync(projectId);

        if (project == null)
            return NotFound();

        return project.StudentId == StudentId ? Ok(project) : Forbid();
    }

    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<IEnumerable<ProjectDto>>>
        GetByStudentId(int studentId)
    {
        if (studentId != StudentId) return Forbid();
        var projects =
            await _projectService.GetByStudentIdAsync(studentId);

        return Ok(projects);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create(
        ProjectDto project)
    {
        project.StudentId = StudentId;
        var created =
            await _projectService.CreateAsync(project);

        return Ok(created);
    }

    [HttpPut("{projectId}")]
    public async Task<ActionResult<ProjectDto>> Update(
        int projectId,
        ProjectDto project)
    {
        var existing = await _projectService.GetByIdAsync(projectId);
        if (existing is null) return NotFound();
        if (existing.StudentId != StudentId) return Forbid();
        project.StudentId = StudentId;
        var updated =
            await _projectService.UpdateAsync(
                projectId,
                project);

        if (updated == null)
            return NotFound();

        return Ok(updated);
    }

    [HttpDelete("{projectId}")]
    public async Task<IActionResult> Delete(int projectId)
    {
        var existing = await _projectService.GetByIdAsync(projectId);
        if (existing is null) return NotFound();
        if (existing.StudentId != StudentId) return Forbid();
        var deleted =
            await _projectService.DeleteAsync(projectId);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
    private int StudentId => int.Parse(User.FindFirstValue("profile_id")!);
}
