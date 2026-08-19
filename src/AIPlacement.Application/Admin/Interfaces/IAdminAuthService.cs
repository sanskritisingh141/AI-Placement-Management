using AIPlacement.Application.Admin.DTOs;

namespace AIPlacement.Application.Admin.Interfaces;

public interface IAdminAuthService
{
    /// <summary>
    /// Validates admin credentials and returns the session info, or null if invalid.
    /// TODO: once Identity (TSK-05) and the shared DbContext are in place, replace the
    /// in-memory check inside AdminAuthService with a real lookup against Users/Roles.
    /// </summary>
    Task<AdminSessionDto?> LoginAsync(AdminLoginRequestDto request);
}
