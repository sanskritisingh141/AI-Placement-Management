using AIPlacement.Application.Authentication.DTOs;

namespace AIPlacement.Application.Authentication.Interfaces;

public interface IAuthService
{
    Task<AuthenticatedUserDto?> LoginAsync(LoginRequestDto request);
    Task<AuthenticatedUserDto> RegisterAsync(
        RegisterRequestDto request,
        bool allowAdminRegistration = false);
}
