using AIPlacement.Application.Students.Interfaces;
using AIPlacement.Domain.Entities.Students;
using AIPlacement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AIPlacement.Infrastructure.Students;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<StudentProfile?> GetByIdAsync(int studentId)
    {
        return await _context.StudentProfiles
            .FirstOrDefaultAsync(x => x.StudentId == studentId);
    }

    public async Task<StudentProfile?> GetByUserIdAsync(int userId)
    {
        return await _context.StudentProfiles
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<StudentProfile> CreateAsync(StudentProfile student)
    {
        _context.StudentProfiles.Add(student);

        await _context.SaveChangesAsync();

        return student;
    }

    public async Task<StudentProfile?> UpdateAsync(
        int studentId,
        StudentProfile student)
    {
        var existing = await _context.StudentProfiles
            .FirstOrDefaultAsync(x => x.StudentId == studentId);

        if (existing == null)
            return null;

        existing.UserId = student.UserId;
        existing.RollNo = student.RollNo;
        existing.Branch = student.Branch;
        existing.CGPA = student.CGPA;
        existing.GraduationYear = student.GraduationYear;
        existing.Phone = student.Phone;
        existing.DateOfBirth = student.DateOfBirth;

        await _context.SaveChangesAsync();

        return existing;
    }

    public async Task<bool> DeleteAsync(int studentId)
    {
        var student = await _context.StudentProfiles
            .FirstOrDefaultAsync(x => x.StudentId == studentId);

        if (student == null)
            return false;

        _context.StudentProfiles.Remove(student);

        await _context.SaveChangesAsync();

        return true;
    }
}