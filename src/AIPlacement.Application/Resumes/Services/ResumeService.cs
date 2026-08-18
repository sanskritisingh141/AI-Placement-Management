using AIPlacement.Application.Resumes.DTOs;
using AIPlacement.Application.Resumes.Interfaces;

namespace AIPlacement.Application.Resumes.Services;

public class ResumeService : IResumeService
{
    public Task<ResumeDto?> GetByIdAsync(int resumeId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ResumeDto>> GetByStudentIdAsync(int studentId)
    {
        throw new NotImplementedException();
    }

    public Task<ResumeDto> CreateAsync(ResumeDto resume)
    {
        throw new NotImplementedException();
    }

    public Task<ResumeDto?> UpdateAsync(int resumeId, ResumeDto resume)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int resumeId)
    {
        throw new NotImplementedException();
    }
}
