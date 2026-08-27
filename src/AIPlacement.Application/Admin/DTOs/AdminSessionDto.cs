namespace AIPlacement.Application.Admin.DTOs;

public class AdminSessionDto
{
    public int UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = "Admin";

    public string Token { get; set; } = string.Empty;
}