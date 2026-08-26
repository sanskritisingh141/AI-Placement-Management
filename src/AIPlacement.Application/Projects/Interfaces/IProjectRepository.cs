using AIPlacement.Application.Projects.DTOs;
using AIPlacement.Domain.Entities.Students;

namespace AIPlacement.Application.Projects.Interfaces;

public interface IProjectRepository
{
    Task<ProjectDto?> GetByIdAsync(int projectId);

    Task<IEnumerable<ProjectDto>> GetByStudentIdAsync(int studentId);

    Task<Project> CreateAsync(Project project);

    Task<Project?> UpdateAsync(
        int projectId,
        Project project);

    Task<bool> DeleteAsync(int projectId);
}