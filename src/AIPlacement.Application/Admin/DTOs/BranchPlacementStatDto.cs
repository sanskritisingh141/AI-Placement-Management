namespace AIPlacement.Application.Admin.DTOs;

public class BranchPlacementStatDto
{
    public string Branch { get; set; } = string.Empty;

    public int TotalStudents { get; set; }

    public int PlacedStudents { get; set; }

    public decimal PlacementPercentage { get; set; }

    public decimal AveragePackage { get; set; }
}
