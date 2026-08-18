using AIPlacement.Application.Students.DTOs;
using AIPlacement.Application.Students.Interfaces;

namespace AIPlacement.Application.Students.Services;

public class StudentService : IStudentService
{
    public Task<StudentDto?> GetByIdAsync(int studentId)
    {
        throw new NotImplementedException();
    }

    public Task<StudentDto?> GetByUserIdAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<StudentDto> CreateAsync(StudentDto student)
    {
        throw new NotImplementedException();
    }

    public Task<StudentDto?> UpdateAsync(int studentId, StudentDto student)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int studentId)
    {
        throw new NotImplementedException();
    }
}