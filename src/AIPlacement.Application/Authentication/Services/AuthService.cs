using AIPlacement.Application.Authentication.DTOs;
using AIPlacement.Application.Authentication.Interfaces;
using AIPlacement.Domain.Entities;
using AIPlacement.Domain.Entities.Students;

namespace AIPlacement.Application.Authentication.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repository;
    private readonly IPasswordHashService _passwords;

    public AuthService(IAuthRepository repository, IPasswordHashService passwords)
    {
        _repository = repository;
        _passwords = passwords;
    }

    public async Task<AuthenticatedUserDto?> LoginAsync(LoginRequestDto request)
    {
        var email = NormalizeEmail(request.Email);
        var user = await _repository.GetByEmailAsync(email);

        if (user is null || !user.IsActive || !_passwords.Verify(request.Password, user.PasswordHash))
            return null;

        return Map(user);
    }

    public async Task<AuthenticatedUserDto> RegisterAsync(
        RegisterRequestDto request,
        bool allowAdminRegistration = false)
    {
        var role = NormalizeRole(request.Role);
        if (role == RoleNames.Admin && !allowAdminRegistration)
            throw new UnauthorizedAccessException("Admin accounts can only be created by an administrator.");

        ValidateRoleSpecificFields(request, role);

        var email = NormalizeEmail(request.Email);
        if (await _repository.EmailExistsAsync(email))
            throw new InvalidOperationException("An account with this email already exists.");

        var roleId = await _repository.GetRoleIdAsync(role)
            ?? throw new InvalidOperationException($"The '{role}' role has not been configured.");

        var user = new User
        {
            RoleId = roleId,
            Name = request.Name.Trim(),
            Email = email,
            PasswordHash = _passwords.Hash(request.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        StudentProfile? student = null;
        CompanyProfile? company = null;

        if (role == RoleNames.Student)
        {
            student = new StudentProfile
            {
                RollNo = request.RollNo!.Trim(),
                Branch = request.Branch!.Trim(),
                CGPA = request.CGPA!.Value,
                GraduationYear = request.GraduationYear!.Value,
                CurrentBacklogs = request.CurrentBacklogs,
                CreatedAt = DateTime.UtcNow
            };
        }
        else if (role == RoleNames.Company)
        {
            company = new CompanyProfile
            {
                CompanyName = request.CompanyName!.Trim(),
                ContactEmail = email
            };
        }

        return Map(await _repository.AddAsync(user, role, student, company));
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static string NormalizeRole(string role)
    {
        var match = RoleNames.All.FirstOrDefault(value =>
            string.Equals(value, role?.Trim(), StringComparison.OrdinalIgnoreCase));

        return match ?? throw new ArgumentException("Role must be Student, Company, or Admin.");
    }

    private static void ValidateRoleSpecificFields(RegisterRequestDto request, string role)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Name is required.");

        if (request.Password.Length < 8)
            throw new ArgumentException("Password must contain at least 8 characters.");

        if (role == RoleNames.Student &&
            (string.IsNullOrWhiteSpace(request.RollNo) ||
             string.IsNullOrWhiteSpace(request.Branch) ||
             request.CGPA is null or < 0 or > 10 ||
             request.GraduationYear is null))
        {
            throw new ArgumentException(
                "Student registration requires roll number, branch, CGPA, and graduation year.");
        }

        if (role == RoleNames.Company && string.IsNullOrWhiteSpace(request.CompanyName))
            throw new ArgumentException("Company registration requires a company name.");
    }

    private static AuthenticatedUserDto Map(AuthUserRecord user) => new()
    {
        UserId = user.UserId,
        Name = user.Name,
        Email = user.Email,
        Role = user.Role,
        ProfileId = user.ProfileId
    };
}
