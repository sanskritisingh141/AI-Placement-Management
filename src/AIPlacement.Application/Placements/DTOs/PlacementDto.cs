namespace AIPlacement.Application.Placements.DTOs;

public class PlacementDto
{
    public int PlacementId { get; set; }

    public int StudentId { get; set; }

    public int ApplicationId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string? RollNo { get; set; }

    public string? Branch { get; set; }

    public int JobDriveId { get; set; }

    public string JobTitle { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public string PlacementStatus { get; set; } = string.Empty;

    public decimal? Package { get; set; }

    public DateOnly? PlacementDate { get; set; }
}
