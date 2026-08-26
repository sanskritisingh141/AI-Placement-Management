using AIPlacement.Application.Certifications.DTOs;
using AIPlacement.Application.Certifications.Interfaces;
using AIPlacement.Domain.Entities.Students;
using AIPlacement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AIPlacement.Infrastructure.Certifications;

public class CertificationRepository : ICertificationRepository
{
    private readonly ApplicationDbContext _context;

    public CertificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CertificationDto?> GetByIdAsync(
        int certificationId)
    {
        return await _context.Certifications
            .Where(c => c.CertificationId == certificationId)
            .Select(c => new CertificationDto
            {
                CertificationId = c.CertificationId,
                StudentId = c.StudentId,
                CertificateName = c.CertificateName,
                IssuingOrganization = c.IssuingOrganization,
                IssueDate = c.IssueDate,
                CredentialUrl = c.CredentialUrl
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<CertificationDto>> GetByStudentIdAsync(
        int studentId)
    {
        return await _context.Certifications
            .Where(c => c.StudentId == studentId)
            .Select(c => new CertificationDto
            {
                CertificationId = c.CertificationId,
                StudentId = c.StudentId,
                CertificateName = c.CertificateName,
                IssuingOrganization = c.IssuingOrganization,
                IssueDate = c.IssueDate,
                CredentialUrl = c.CredentialUrl
            })
            .ToListAsync();
    }

    public async Task<Certification> CreateAsync(
        Certification certification)
    {
        _context.Certifications.Add(certification);

        await _context.SaveChangesAsync();

        return certification;
    }

    public async Task<Certification?> UpdateAsync(
        int certificationId,
        Certification certification)
    {
        var existing = await _context.Certifications
            .FirstOrDefaultAsync(
                c => c.CertificationId == certificationId);

        if (existing == null)
            return null;

        existing.StudentId = certification.StudentId;
        existing.CertificateName = certification.CertificateName;
        existing.IssuingOrganization =
            certification.IssuingOrganization;
        existing.IssueDate = certification.IssueDate;
        existing.CredentialUrl = certification.CredentialUrl;

        await _context.SaveChangesAsync();

        return existing;
    }

    public async Task<bool> DeleteAsync(int certificationId)
    {
        var certification = await _context.Certifications
            .FirstOrDefaultAsync(
                c => c.CertificationId == certificationId);

        if (certification == null)
            return false;

        _context.Certifications.Remove(certification);

        await _context.SaveChangesAsync();

        return true;
    }
}