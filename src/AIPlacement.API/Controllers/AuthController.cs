using AIPlacement.API.Security;
using AIPlacement.Application.Authentication;
using AIPlacement.Application.Authentication.DTOs;
using AIPlacement.Application.Authentication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly JwtTokenService _tokens;

    public AuthController(IAuthService authService, JwtTokenService tokens)
    {
        _authService = authService;
        _tokens = tokens;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        var user = await _authService.LoginAsync(request);
        if (user is null)
            return Unauthorized(new { message = "Invalid credentials or inactive account." });

        var token = _tokens.Create(user);
        return Ok(new
        {
            token = token.Token,
            expiresAt = token.ExpiresAt,
            user
        });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto request)
    {
        if (string.Equals(request.Role, RoleNames.Admin, StringComparison.OrdinalIgnoreCase))
            return Forbid();

        return await RegisterCore(request, allowAdmin: false);
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost("register-admin")]
    public Task<IActionResult> RegisterAdmin(RegisterRequestDto request)
    {
        request.Role = RoleNames.Admin;
        return RegisterCore(request, allowAdmin: true);
    }

    private async Task<IActionResult> RegisterCore(RegisterRequestDto request, bool allowAdmin)
    {
        try
        {
            var user = await _authService.RegisterAsync(request, allowAdmin);
            var token = _tokens.Create(user);
            return Ok(new { token = token.Token, expiresAt = token.ExpiresAt, user });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }
}
