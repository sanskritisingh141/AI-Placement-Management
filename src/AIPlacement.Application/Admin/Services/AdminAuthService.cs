using AIPlacement.Application.Admin.DTOs;
using AIPlacement.Application.Admin.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AIPlacement.Application.Admin.Services;

public class AdminAuthService : IAdminAuthService
{
    private readonly IConfiguration _configuration;

    private const string SeedAdminEmail = "admin@aiplacement.edu";
    private const string SeedAdminPassword = "Admin@123";

    public AdminAuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<AdminSessionDto?> LoginAsync(AdminLoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return Task.FromResult<AdminSessionDto?>(null);
        }

        // Admin credentials
        bool isAdmin =
            string.Equals(
                request.Email,
                SeedAdminEmail,
                StringComparison.OrdinalIgnoreCase) &&
            request.Password == SeedAdminPassword;

        // Student credentials
        const string StudentEmail = "student@aiplacement.edu";
        const string StudentPassword = "Student@123";

        bool isStudent =
            string.Equals(
                request.Email,
                StudentEmail,
                StringComparison.OrdinalIgnoreCase) &&
            request.Password == StudentPassword;

        // Invalid credentials
        if (!isAdmin && !isStudent)
            return Task.FromResult<AdminSessionDto?>(null);

        // JWT configuration
        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];

        if (string.IsNullOrWhiteSpace(jwtKey))
            throw new InvalidOperationException("JWT signing key is missing.");

        // Set user information based on role
        var userId = isAdmin ? "1" : "2";
        var name = isAdmin ? "Placement Admin" : "Student Test";
        var email = isAdmin ? SeedAdminEmail : StudentEmail;
        var role = isAdmin ? "Admin" : "Student";

        // Claims
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        // Security key
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        // Create JWT token
        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        // Convert token to string
        var tokenString = new JwtSecurityTokenHandler()
            .WriteToken(token);

        // Return session
        var session = new AdminSessionDto
        {
            UserId = int.Parse(userId),
            Name = name,
            Email = email,
            Role = role,
            Token = tokenString
        };

        return Task.FromResult<AdminSessionDto?>(session);
    }
}