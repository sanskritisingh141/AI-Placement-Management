using AIPlacement.Application.Certifications.DTOs;
using AIPlacement.Domain.Entities.Students;

namespace AIPlacement.Application.Certifications.Interfaces;

public interface ICertificationRepository
{
    Task<CertificationDto?> GetByIdAsync(int certificationId);

    Task<IEnumerable<CertificationDto>> GetByStudentIdAsync(int studentId);

    Task<Certification> CreateAsync(Certification certification);

    Task<Certification?> UpdateAsync(
        int certificationId,
        Certification certification);

    Task<bool> DeleteAsync(int certificationId);
}
