using System.ComponentModel.DataAnnotations;

namespace AIPlacement.Application.Authentication.DTOs;

public class RegisterRequestDto
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8), MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;

    [StringLength(50)]
    public string? RollNo { get; set; }

    [StringLength(100)]
    public string? Branch { get; set; }

    [Range(0, 10)]
    public decimal? CGPA { get; set; }

    [Range(2000, 2200)]
    public int? GraduationYear { get; set; }

    [Range(0, 100)]
    public int CurrentBacklogs { get; set; }

    [StringLength(200)]
    public string? CompanyName { get; set; }
}
