using AIPlacement.Application.Admin.DTOs;
using AIPlacement.Application.Admin.Interfaces;

namespace AIPlacement.Application.Admin.Services;

public class AdminAuthService : IAdminAuthService
{
    // TODO(Shared Foundation): replace with a lookup against Users/Roles once
    // Identity + EF Core (TSK-04, TSK-05) are ready. Kept here so the Admin module
    // is independently testable in the meantime.
    private const string SeedAdminEmail = "admin@aiplacement.edu";
    private const string SeedAdminPassword = "Admin@123";

    public Task<AdminSessionDto?> LoginAsync(AdminLoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return Task.FromResult<AdminSessionDto?>(null);

        var isValid =
            string.Equals(request.Email, SeedAdminEmail, StringComparison.OrdinalIgnoreCase) &&
            request.Password == SeedAdminPassword;

        if (!isValid)
            return Task.FromResult<AdminSessionDto?>(null);

        var session = new AdminSessionDto
        {
            UserId = 1,
            Name = "Placement Admin",
            Email = SeedAdminEmail,
            Role = "Admin"
        };

        return Task.FromResult<AdminSessionDto?>(session);
    }
}
