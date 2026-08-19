namespace AIPlacement.Application.Admin.DTOs;

public class UserRecordDto
{
    public int UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    // Populated only when Role == "Student"
    public string? RollNo { get; set; }

    public string? Branch { get; set; }

    public decimal? CGPA { get; set; }

    // Populated only when Role == "Company"
    public string? CompanyName { get; set; }

    public string? Industry { get; set; }
}
