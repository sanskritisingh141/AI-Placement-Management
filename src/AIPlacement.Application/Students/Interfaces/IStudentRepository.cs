using AIPlacement.Domain.Entities.Students;

namespace AIPlacement.Application.Students.Interfaces;

public interface IStudentRepository
{
    Task<StudentProfile?> GetByIdAsync(int studentId);

    Task<StudentProfile?> GetByUserIdAsync(int userId);

    Task<StudentProfile> CreateAsync(StudentProfile student);

    Task<StudentProfile?> UpdateAsync(int studentId, StudentProfile student);

    Task<bool> DeleteAsync(int studentId);
}