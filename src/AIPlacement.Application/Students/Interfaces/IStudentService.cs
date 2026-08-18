using AIPlacement.Application.Students.DTOs;

namespace AIPlacement.Application.Students.Interfaces;

public interface IStudentService
{
    Task<StudentDto?> GetByIdAsync(int studentId);

    Task<StudentDto?> GetByUserIdAsync(int userId);

    Task<StudentDto> CreateAsync(StudentDto student);

    Task<StudentDto?> UpdateAsync(int studentId, StudentDto student);

    Task<bool> DeleteAsync(int studentId);
}
