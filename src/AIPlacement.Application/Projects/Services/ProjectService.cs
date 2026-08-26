using AIPlacement.Application.Projects.DTOs;
using AIPlacement.Application.Projects.Interfaces;
using AIPlacement.Domain.Entities.Students;

namespace AIPlacement.Application.Projects.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ProjectDto?> GetByIdAsync(int projectId)
    {
        return await _projectRepository.GetByIdAsync(projectId);
    }

    public async Task<IEnumerable<ProjectDto>> GetByStudentIdAsync(
        int studentId)
    {
        return await _projectRepository.GetByStudentIdAsync(studentId);
    }

    public async Task<ProjectDto> CreateAsync(ProjectDto project)
    {
        var entity = new Project
        {
            StudentId = project.StudentId,
            ProjectTitle = project.ProjectTitle ?? string.Empty,
            Description = project.Description,
            TechnologiesUsed = project.TechnologiesUsed,
            ProjectUrl = project.ProjectUrl,
            CreatedAt = project.CreatedAt ?? DateTime.UtcNow
        };

        var created =
            await _projectRepository.CreateAsync(entity);

        return MapToDto(created);
    }

    public async Task<ProjectDto?> UpdateAsync(
        int projectId,
        ProjectDto project)
    {
        var entity = new Project
        {
            ProjectId = projectId,
            StudentId = project.StudentId,
            ProjectTitle = project.ProjectTitle ?? string.Empty,
            Description = project.Description,
            TechnologiesUsed = project.TechnologiesUsed,
            ProjectUrl = project.ProjectUrl,
            CreatedAt = project.CreatedAt ?? DateTime.UtcNow
        };

        var updated =
            await _projectRepository.UpdateAsync(
                projectId,
                entity);

        if (updated == null)
            return null;

        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(int projectId)
    {
        return await _projectRepository.DeleteAsync(projectId);
    }

    private static ProjectDto MapToDto(Project project)
    {
        return new ProjectDto
        {
            ProjectId = project.ProjectId,
            StudentId = project.StudentId,
            ProjectTitle = project.ProjectTitle,
            Description = project.Description,
            TechnologiesUsed = project.TechnologiesUsed,
            ProjectUrl = project.ProjectUrl,
            CreatedAt = project.CreatedAt
        };
    }
}
