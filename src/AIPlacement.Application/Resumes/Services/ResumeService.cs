using AIPlacement.Application.Resumes.DTOs;
using AIPlacement.Application.Resumes.Interfaces;
using AIPlacement.Domain.Entities.Resumes;

namespace AIPlacement.Application.Resumes.Services;

public class ResumeService : IResumeService
{
    private readonly IResumeRepository _resumeRepository;

    public ResumeService(IResumeRepository resumeRepository)
    {
        _resumeRepository = resumeRepository;
    }

    public async Task<ResumeDto?> GetByIdAsync(int resumeId)
    {
        return await _resumeRepository.GetByIdAsync(resumeId);
    }

    public async Task<IEnumerable<ResumeDto>> GetByStudentIdAsync(int studentId)
    {
        return await _resumeRepository.GetByStudentIdAsync(studentId);
    }

    public async Task<ResumeDto> CreateAsync(ResumeDto resume)
    {
        var entity = new Resume
        {
            StudentId = resume.StudentId,
            FileName = resume.FileName ?? string.Empty,
            FilePath = resume.FilePath ?? string.Empty,
            UploadedAt = resume.UploadedAt ?? DateTime.UtcNow,
            VersionNo = resume.VersionNo ?? 1,
            IsCurrent = resume.IsCurrent ?? true
        };

        var created = await _resumeRepository.CreateAsync(entity);

        return MapToDto(created);
    }

    public async Task<ResumeDto> AddVersionAsync(
        int studentId,
        string fileName,
        string filePath)
    {
        if (studentId <= 0)
            throw new ArgumentException("A valid student ID is required.");

        if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Resume file information is required.");

        return MapToDto(await _resumeRepository.AddVersionAsync(
            studentId,
            fileName.Trim(),
            filePath.Trim()));
    }

    public async Task<ResumeDto?> UpdateAsync(
        int resumeId,
        ResumeDto resume)
    {
        var entity = new Resume
        {
            ResumeId = resumeId,
            StudentId = resume.StudentId,
            FileName = resume.FileName ?? string.Empty,
            FilePath = resume.FilePath ?? string.Empty,
            UploadedAt = resume.UploadedAt ?? DateTime.UtcNow,
            VersionNo = resume.VersionNo ?? 1,
            IsCurrent = resume.IsCurrent ?? true
        };

        var updated = await _resumeRepository.UpdateAsync(
            resumeId,
            entity);

        if (updated == null)
            return null;

        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(int resumeId)
    {
        return await _resumeRepository.DeleteAsync(resumeId);
    }

    private static ResumeDto MapToDto(Resume resume)
    {
        return new ResumeDto
        {
            ResumeId = resume.ResumeId,
            StudentId = resume.StudentId,
            FileName = resume.FileName,
            FilePath = resume.FilePath,
            UploadedAt = resume.UploadedAt,
            VersionNo = resume.VersionNo,
            IsCurrent = resume.IsCurrent
        };
    }
}
