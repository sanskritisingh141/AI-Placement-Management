using AIPlacement.Application.Resumes.DTOs;

namespace AIPlacement.Application.Resumes.Interfaces;

public interface IResumeService
{
    Task<ResumeDto?> GetByIdAsync(int resumeId);

    Task<IEnumerable<ResumeDto>> GetByStudentIdAsync(int studentId);

    Task<ResumeDto> CreateAsync(ResumeDto resume);

    Task<ResumeDto?> UpdateAsync(int resumeId, ResumeDto resume);

    Task<bool> DeleteAsync(int resumeId);
}