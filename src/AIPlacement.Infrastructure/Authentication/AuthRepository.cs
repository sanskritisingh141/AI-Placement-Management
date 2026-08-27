using AIPlacement.Application.Authentication;
using AIPlacement.Application.Authentication.DTOs;
using AIPlacement.Application.Authentication.Interfaces;
using AIPlacement.Domain.Entities;
using AIPlacement.Domain.Entities.Students;
using AIPlacement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AIPlacement.Infrastructure.Authentication;

public class AuthRepository : IAuthRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AuthRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AuthUserRecord?> GetByEmailAsync(string normalizedEmail)
    {
        var record = await (
            from user in _dbContext.Users.AsNoTracking()
            join role in _dbContext.Roles.AsNoTracking() on user.RoleId equals role.RoleId
            where user.Email == normalizedEmail
            select new AuthUserRecord
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Role = role.RoleName,
                IsActive = user.IsActive
            }).SingleOrDefaultAsync();

        if (record is null)
            return null;

        record.ProfileId = await GetProfileIdAsync(record.UserId, record.Role);
        return record;
    }

    public Task<bool> EmailExistsAsync(string normalizedEmail) =>
        _dbContext.Users.AnyAsync(user => user.Email == normalizedEmail);

    public Task<int?> GetRoleIdAsync(string roleName) =>
        _dbContext.Roles
            .Where(role => role.RoleName == roleName)
            .Select(role => (int?)role.RoleId)
            .SingleOrDefaultAsync();

    public async Task<AuthUserRecord> AddAsync(
        User user,
        string roleName,
        StudentProfile? studentProfile,
        CompanyProfile? companyProfile)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        int? profileId = null;
        if (studentProfile is not null)
        {
            studentProfile.UserId = user.UserId;
            _dbContext.StudentProfiles.Add(studentProfile);
            await _dbContext.SaveChangesAsync();
            profileId = studentProfile.StudentId;
        }
        else if (companyProfile is not null)
        {
            companyProfile.UserId = user.UserId;
            _dbContext.CompanyProfiles.Add(companyProfile);
            await _dbContext.SaveChangesAsync();
            profileId = companyProfile.CompanyId;
        }

        await transaction.CommitAsync();

        return new AuthUserRecord
        {
            UserId = user.UserId,
            Name = user.Name,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            Role = roleName,
            IsActive = user.IsActive,
            ProfileId = profileId
        };
    }

    private async Task<int?> GetProfileIdAsync(int userId, string role)
    {
        if (role == RoleNames.Student)
        {
            return await _dbContext.StudentProfiles
                .Where(profile => profile.UserId == userId)
                .Select(profile => (int?)profile.StudentId)
                .SingleOrDefaultAsync();
        }

        if (role == RoleNames.Company)
        {
            return await _dbContext.CompanyProfiles
                .Where(profile => profile.UserId == userId)
                .Select(profile => (int?)profile.CompanyId)
                .SingleOrDefaultAsync();
        }

        return null;
    }
}
