using AIPlacement.Application.Resumes.DTOs;
using AIPlacement.Domain.Entities.Resumes;

namespace AIPlacement.Application.Resumes.Interfaces;

public interface IResumeRepository
{
    Task<ResumeDto?> GetByIdAsync(int resumeId);

    Task<IEnumerable<ResumeDto>> GetByStudentIdAsync(int studentId);

    Task<Resume> CreateAsync(Resume resume);

    Task<Resume?> UpdateAsync(
        int resumeId,
        Resume resume);

    Task<bool> DeleteAsync(int resumeId);
}
