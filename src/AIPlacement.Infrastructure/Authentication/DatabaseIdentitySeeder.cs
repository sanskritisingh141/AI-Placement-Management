using AIPlacement.Application.Authentication;
using AIPlacement.Application.Authentication.Interfaces;
using AIPlacement.Domain.Entities;
using AIPlacement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AIPlacement.Infrastructure.Authentication;

public class DatabaseIdentitySeeder
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHashService _passwords;

    public DatabaseIdentitySeeder(
        ApplicationDbContext dbContext,
        IPasswordHashService passwords)
    {
        _dbContext = dbContext;
        _passwords = passwords;
    }

    public async Task SeedAsync(string? adminEmail, string? adminPassword)
    {
        foreach (var roleName in RoleNames.All)
        {
            if (!await _dbContext.Roles.AnyAsync(role => role.RoleName == roleName))
                _dbContext.Roles.Add(new Role { RoleName = roleName });
        }

        await _dbContext.SaveChangesAsync();

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            return;

        var normalizedEmail = adminEmail.Trim().ToLowerInvariant();
        if (await _dbContext.Users.AnyAsync(user => user.Email == normalizedEmail))
            return;

        var adminRoleId = await _dbContext.Roles
            .Where(role => role.RoleName == RoleNames.Admin)
            .Select(role => role.RoleId)
            .SingleAsync();

        _dbContext.Users.Add(new User
        {
            RoleId = adminRoleId,
            Name = "Placement Admin",
            Email = normalizedEmail,
            PasswordHash = _passwords.Hash(adminPassword),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();
    }
}
