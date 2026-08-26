using AIPlacement.Application.Certifications.DTOs;

namespace AIPlacement.Application.Certifications.Interfaces;

public interface ICertificationService
{
    Task<CertificationDto?> GetByIdAsync(int certificationId);

    Task<IEnumerable<CertificationDto>> GetByStudentIdAsync(int studentId);

    Task<CertificationDto> CreateAsync(CertificationDto certification);

    Task<CertificationDto?> UpdateAsync(
        int certificationId,
        CertificationDto certification);

    Task<bool> DeleteAsync(int certificationId);
}