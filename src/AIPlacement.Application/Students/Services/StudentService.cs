using AIPlacement.Application.Students.DTOs;
using AIPlacement.Application.Students.Interfaces;
using AIPlacement.Domain.Entities.Students;

namespace AIPlacement.Application.Students.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;

    public StudentService(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<StudentDto?> GetByIdAsync(int studentId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);

        if (student == null)
            return null;

        return MapToDto(student);
    }

    public async Task<StudentDto?> GetByUserIdAsync(int userId)
    {
        var student = await _studentRepository.GetByUserIdAsync(userId);

        if (student == null)
            return null;

        return MapToDto(student);
    }

    public async Task<StudentDto> CreateAsync(StudentDto student)
    {
        var entity = new StudentProfile
        {
            UserId = student.UserId,
            RollNo = student.RollNo ?? string.Empty,
            Branch = student.Branch ?? string.Empty,
            CGPA = student.CGPA ?? 0,
            GraduationYear = student.GraduationYear ?? 0,
            Phone = student.Phone,
            DateOfBirth = student.DateOfBirth,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _studentRepository.CreateAsync(entity);

        return MapToDto(created);
    }

    public async Task<StudentDto?> UpdateAsync(
        int studentId,
        StudentDto student)
    {
        var entity = new StudentProfile
        {
            StudentId = studentId,
            UserId = student.UserId,
            RollNo = student.RollNo ?? string.Empty,
            Branch = student.Branch ?? string.Empty,
            CGPA = student.CGPA ?? 0,
            GraduationYear = student.GraduationYear ?? 0,
            Phone = student.Phone,
            DateOfBirth = student.DateOfBirth
        };

        var updated =
            await _studentRepository.UpdateAsync(studentId, entity);

        if (updated == null)
            return null;

        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(int studentId)
    {
        return await _studentRepository.DeleteAsync(studentId);
    }

    private static StudentDto MapToDto(StudentProfile student)
    {
        return new StudentDto
        {
            StudentId = student.StudentId,
            UserId = student.UserId,
            RollNo = student.RollNo,
            Branch = student.Branch,
            CGPA = student.CGPA,
            GraduationYear = student.GraduationYear,
            Phone = student.Phone,
            DateOfBirth = student.DateOfBirth
        };
    }
}