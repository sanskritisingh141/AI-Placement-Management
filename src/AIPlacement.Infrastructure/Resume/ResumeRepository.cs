using AIPlacement.Application.Resumes.DTOs;
using AIPlacement.Application.Resumes.Interfaces;
using AIPlacement.Domain.Entities.Resumes;
using AIPlacement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AIPlacement.Infrastructure.Resumes;

public class ResumeRepository : IResumeRepository
{
    private readonly ApplicationDbContext _context;

    public ResumeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResumeDto?> GetByIdAsync(int resumeId)
    {
        return await _context.Resumes
            .Where(r => r.ResumeId == resumeId)
            .Select(r => new ResumeDto
            {
                ResumeId = r.ResumeId,
                StudentId = r.StudentId,
                FileName = r.FileName,
                FilePath = r.FilePath,
                UploadedAt = r.UploadedAt,
                VersionNo = r.VersionNo,
                IsCurrent = r.IsCurrent
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ResumeDto>> GetByStudentIdAsync(int studentId)
    {
        return await _context.Resumes
            .Where(r => r.StudentId == studentId)
            .Select(r => new ResumeDto
            {
                ResumeId = r.ResumeId,
                StudentId = r.StudentId,
                FileName = r.FileName,
                FilePath = r.FilePath,
                UploadedAt = r.UploadedAt,
                VersionNo = r.VersionNo,
                IsCurrent = r.IsCurrent
            })
            .ToListAsync();
    }

    public async Task<Resume> CreateAsync(Resume resume)
    {
        _context.Resumes.Add(resume);

        await _context.SaveChangesAsync();

        return resume;
    }

    public async Task<Resume> AddVersionAsync(
        int studentId,
        string fileName,
        string filePath)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        var existing = await _context.Resumes
            .Where(resume => resume.StudentId == studentId)
            .ToListAsync();

        foreach (var resume in existing)
            resume.IsCurrent = false;

        var version = existing.Count == 0
            ? 1
            : existing.Max(resume => resume.VersionNo) + 1;

        var created = new Resume
        {
            StudentId = studentId,
            FileName = fileName,
            FilePath = filePath,
            UploadedAt = DateTime.UtcNow,
            VersionNo = version,
            IsCurrent = true
        };

        _context.Resumes.Add(created);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return created;
    }

    public async Task<Resume?> UpdateAsync(
        int resumeId,
        Resume resume)
    {
        var existing = await _context.Resumes
            .FirstOrDefaultAsync(r => r.ResumeId == resumeId);

        if (existing == null)
            return null;

        existing.StudentId = resume.StudentId;
        existing.FileName = resume.FileName;
        existing.FilePath = resume.FilePath;
        existing.UploadedAt = resume.UploadedAt;
        existing.VersionNo = resume.VersionNo;
        existing.IsCurrent = resume.IsCurrent;

        await _context.SaveChangesAsync();

        return existing;
    }

    public async Task<bool> DeleteAsync(int resumeId)
    {
        var resume = await _context.Resumes
            .FirstOrDefaultAsync(r => r.ResumeId == resumeId);

        if (resume == null)
            return false;

        _context.Resumes.Remove(resume);

        await _context.SaveChangesAsync();

        return true;
    }
}
