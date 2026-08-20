namespace AIPlacement.Application.Company.DTOs;

public class CompanyProfileDto
{
    public int CompanyId { get; set; }

    public int UserId { get; set; }

    public string CompanyName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Website { get; set; }

    public string? Industry { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactPhone { get; set; }
}