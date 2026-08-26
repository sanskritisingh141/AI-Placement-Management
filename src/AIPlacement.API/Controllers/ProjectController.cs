using AIPlacement.Application.Projects.DTOs;
using AIPlacement.Application.Projects.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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

        return Ok(project);
    }

    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<IEnumerable<ProjectDto>>>
        GetByStudentId(int studentId)
    {
        var projects =
            await _projectService.GetByStudentIdAsync(studentId);

        return Ok(projects);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create(
        ProjectDto project)
    {
        var created =
            await _projectService.CreateAsync(project);

        return Ok(created);
    }

    [HttpPut("{projectId}")]
    public async Task<ActionResult<ProjectDto>> Update(
        int projectId,
        ProjectDto project)
    {
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
        var deleted =
            await _projectService.DeleteAsync(projectId);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}