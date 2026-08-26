using AIPlacement.Application.Certifications.DTOs;
using AIPlacement.Application.Certifications.Interfaces;
using AIPlacement.Domain.Entities.Students;

namespace AIPlacement.Application.Certifications.Services;

public class CertificationService : ICertificationService
{
    private readonly ICertificationRepository _certificationRepository;

    public CertificationService(
        ICertificationRepository certificationRepository)
    {
        _certificationRepository = certificationRepository;
    }

    public async Task<CertificationDto?> GetByIdAsync(
        int certificationId)
    {
        return await _certificationRepository
            .GetByIdAsync(certificationId);
    }

    public async Task<IEnumerable<CertificationDto>>
        GetByStudentIdAsync(int studentId)
    {
        return await _certificationRepository
            .GetByStudentIdAsync(studentId);
    }

    public async Task<CertificationDto> CreateAsync(
        CertificationDto certification)
    {
        var entity = new Certification
        {
            StudentId = certification.StudentId,
            CertificateName =
                certification.CertificateName ?? string.Empty,
            IssuingOrganization =
                certification.IssuingOrganization ?? string.Empty,
            IssueDate =
                certification.IssueDate ?? DateTime.UtcNow,
            CredentialUrl = certification.CredentialUrl
        };

        var created =
            await _certificationRepository.CreateAsync(entity);

        return MapToDto(created);
    }

    public async Task<CertificationDto?> UpdateAsync(
        int certificationId,
        CertificationDto certification)
    {
        var entity = new Certification
        {
            CertificationId = certificationId,
            StudentId = certification.StudentId,
            CertificateName =
                certification.CertificateName ?? string.Empty,
            IssuingOrganization =
                certification.IssuingOrganization ?? string.Empty,
            IssueDate =
                certification.IssueDate ?? DateTime.UtcNow,
            CredentialUrl = certification.CredentialUrl
        };

        var updated =
            await _certificationRepository.UpdateAsync(
                certificationId,
                entity);

        if (updated == null)
            return null;

        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(int certificationId)
    {
        return await _certificationRepository
            .DeleteAsync(certificationId);
    }

    private static CertificationDto MapToDto(
        Certification certification)
    {
        return new CertificationDto
        {
            CertificationId = certification.CertificationId,
            StudentId = certification.StudentId,
            CertificateName = certification.CertificateName,
            IssuingOrganization =
                certification.IssuingOrganization,
            IssueDate = certification.IssueDate,
            CredentialUrl = certification.CredentialUrl
        };
    }
}