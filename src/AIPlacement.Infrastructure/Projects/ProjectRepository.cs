using AIPlacement.Application.Projects.DTOs;
using AIPlacement.Application.Projects.Interfaces;
using AIPlacement.Domain.Entities.Students;
using AIPlacement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AIPlacement.Infrastructure.Projects;

public class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProjectDto?> GetByIdAsync(int projectId)
    {
        return await _context.Projects
            .Where(p => p.ProjectId == projectId)
            .Select(p => new ProjectDto
            {
                ProjectId = p.ProjectId,
                StudentId = p.StudentId,
                ProjectTitle = p.ProjectTitle,
                Description = p.Description,
                TechnologiesUsed = p.TechnologiesUsed,
                ProjectUrl = p.ProjectUrl,
                CreatedAt = p.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ProjectDto>> GetByStudentIdAsync(
        int studentId)
    {
        return await _context.Projects
            .Where(p => p.StudentId == studentId)
            .Select(p => new ProjectDto
            {
                ProjectId = p.ProjectId,
                StudentId = p.StudentId,
                ProjectTitle = p.ProjectTitle,
                Description = p.Description,
                TechnologiesUsed = p.TechnologiesUsed,
                ProjectUrl = p.ProjectUrl,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<Project> CreateAsync(Project project)
    {
        _context.Projects.Add(project);

        await _context.SaveChangesAsync();

        return project;
    }

    public async Task<Project?> UpdateAsync(
        int projectId,
        Project project)
    {
        var existing = await _context.Projects
            .FirstOrDefaultAsync(p => p.ProjectId == projectId);

        if (existing == null)
            return null;

        existing.StudentId = project.StudentId;
        existing.ProjectTitle = project.ProjectTitle;
        existing.Description = project.Description;
        existing.TechnologiesUsed = project.TechnologiesUsed;
        existing.ProjectUrl = project.ProjectUrl;

        await _context.SaveChangesAsync();

        return existing;
    }

    public async Task<bool> DeleteAsync(int projectId)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.ProjectId == projectId);

        if (project == null)
            return false;

        _context.Projects.Remove(project);

        await _context.SaveChangesAsync();

        return true;
    }
}