using AIPlacement.Application.Authentication;
using AIPlacement.Application.Authentication.DTOs;
using AIPlacement.Application.Authentication.Interfaces;
using AIPlacement.Application.Authentication.Services;
using AIPlacement.Domain.Entities;
using AIPlacement.Domain.Entities.Students;

namespace AIPlacement.Tests.Authentication;

public class AuthServiceTests
{
    [Fact]
    public void PasswordHash_RoundTripsWithoutStoringPlainText()
    {
        var passwords = new Pbkdf2PasswordHashService();
        var hash = passwords.Hash("Student@123");

        Assert.DoesNotContain("Student@123", hash);
        Assert.True(passwords.Verify("Student@123", hash));
        Assert.False(passwords.Verify("WrongPassword", hash));
    }

    [Fact]
    public async Task Login_RejectsInactiveUser()
    {
        var passwords = new Pbkdf2PasswordHashService();
        var repository = new AuthRepositoryStub
        {
            Existing = new AuthUserRecord
            {
                UserId = 1,
                Email = "student@example.com",
                PasswordHash = passwords.Hash("Student@123"),
                Role = RoleNames.Student,
                IsActive = false
            }
        };

        var result = await new AuthService(repository, passwords).LoginAsync(new LoginRequestDto
        {
            Email = "student@example.com",
            Password = "Student@123"
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task PublicRegistration_RejectsAdminRole()
    {
        var service = new AuthService(new AuthRepositoryStub(), new Pbkdf2PasswordHashService());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.RegisterAsync(new RegisterRequestDto
            {
                Name = "Admin",
                Email = "admin@example.com",
                Password = "Admin@123",
                Role = RoleNames.Admin
            }));
    }

    [Fact]
    public async Task StudentRegistration_CreatesStudentProfile()
    {
        var repository = new AuthRepositoryStub { RoleId = 1 };
        var service = new AuthService(repository, new Pbkdf2PasswordHashService());

        var result = await service.RegisterAsync(new RegisterRequestDto
        {
            Name = "Aarav Sharma",
            Email = "AARAV@example.com",
            Password = "Student@123",
            Role = RoleNames.Student,
            RollNo = "22CSE001",
            Branch = "CSE",
            CGPA = 8.4m,
            GraduationYear = 2027
        });

        Assert.Equal("aarav@example.com", result.Email);
        Assert.NotNull(repository.AddedStudent);
        Assert.Equal("22CSE001", repository.AddedStudent!.RollNo);
    }

    private sealed class AuthRepositoryStub : IAuthRepository
    {
        public AuthUserRecord? Existing { get; init; }
        public int? RoleId { get; init; }
        public StudentProfile? AddedStudent { get; private set; }

        public Task<AuthUserRecord?> GetByEmailAsync(string normalizedEmail) =>
            Task.FromResult(Existing);

        public Task<bool> EmailExistsAsync(string normalizedEmail) =>
            Task.FromResult(Existing is not null);

        public Task<int?> GetRoleIdAsync(string roleName) => Task.FromResult(RoleId);

        public Task<AuthUserRecord> AddAsync(
            User user,
            string roleName,
            StudentProfile? studentProfile,
            CompanyProfile? companyProfile)
        {
            AddedStudent = studentProfile;
            return Task.FromResult(new AuthUserRecord
            {
                UserId = 10,
                Name = user.Name,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Role = roleName,
                IsActive = true,
                ProfileId = studentProfile is null ? null : 20
            });
        }
    }
}
