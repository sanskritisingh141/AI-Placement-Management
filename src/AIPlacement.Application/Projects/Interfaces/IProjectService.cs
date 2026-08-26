using AIPlacement.Application.Projects.DTOs;

namespace AIPlacement.Application.Projects.Interfaces;

public interface IProjectService
{
    Task<ProjectDto?> GetByIdAsync(int projectId);

    Task<IEnumerable<ProjectDto>> GetByStudentIdAsync(int studentId);

    Task<ProjectDto> CreateAsync(ProjectDto project);

    Task<ProjectDto?> UpdateAsync(
        int projectId,
        ProjectDto project);

    Task<bool> DeleteAsync(int projectId);
}