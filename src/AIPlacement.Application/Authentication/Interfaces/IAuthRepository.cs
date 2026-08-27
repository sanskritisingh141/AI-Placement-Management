using AIPlacement.Application.Authentication.DTOs;
using AIPlacement.Domain.Entities;
using AIPlacement.Domain.Entities.Students;

namespace AIPlacement.Application.Authentication.Interfaces;

public interface IAuthRepository
{
    Task<AuthUserRecord?> GetByEmailAsync(string normalizedEmail);
    Task<bool> EmailExistsAsync(string normalizedEmail);
    Task<int?> GetRoleIdAsync(string roleName);
    Task<AuthUserRecord> AddAsync(
        User user,
        string roleName,
        StudentProfile? studentProfile,
        CompanyProfile? companyProfile);
}
