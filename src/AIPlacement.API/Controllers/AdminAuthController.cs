using AIPlacement.Application.Admin.DTOs;
using AIPlacement.Application.Admin.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/admin/auth")]
public class AdminAuthController : ControllerBase
{
    private readonly IAdminAuthService _adminAuthService;

    public AdminAuthController(IAdminAuthService adminAuthService)
    {
        _adminAuthService = adminAuthService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AdminLoginRequestDto request)
    {
        var session = await _adminAuthService.LoginAsync(request);

        if (session == null)
            return Unauthorized(new { message = "Invalid admin credentials." });

        return Ok(session);
    }
}
