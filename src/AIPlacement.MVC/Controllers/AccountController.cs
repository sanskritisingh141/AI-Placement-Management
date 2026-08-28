using System.Security.Claims;
using AIPlacement.Application.Authentication;
using AIPlacement.Application.Authentication.DTOs;
using AIPlacement.Application.Authentication.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.MVC.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectForRole(User);

        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginRequestDto());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequestDto request, string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
            return View(request);

        var user = await _authService.LoginAsync(request);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid credentials or inactive account.");
            return View(request);
        }

        await SignInAsync(user);

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectForRole(UserFrom(user));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register() => View(new RegisterRequestDto());

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterRequestDto request)
    {
        if (string.Equals(request.Role, RoleNames.Admin, StringComparison.OrdinalIgnoreCase))
            ModelState.AddModelError(nameof(request.Role), "Admin accounts are created by an administrator.");

        if (!ModelState.IsValid)
            return View(request);

        try
        {
            var user = await _authService.RegisterAsync(request);
            await SignInAsync(user);
            return RedirectForRole(UserFrom(user));
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
        }

        return View(request);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied() => View();

    private async Task SignInAsync(AuthenticatedUserDto user)
    {
        var principal = UserFrom(user);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
            });
    }

    private static ClaimsPrincipal UserFrom(AuthenticatedUserDto user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        if (user.ProfileId.HasValue)
            claims.Add(new Claim("profile_id", user.ProfileId.Value.ToString()));

        return new ClaimsPrincipal(
            new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
    }

    private IActionResult RedirectForRole(ClaimsPrincipal principal)
    {
        if (principal.IsInRole(RoleNames.Admin))
            return RedirectToAction("Dashboard", "Admin");

        if (principal.IsInRole(RoleNames.Company))
            return RedirectToAction("Index", "CompanyDashboard");

        return RedirectToAction("Index", "StudentDashboard");
    }
}
