namespace AIPlacement.Application.Admin.DTOs;

public class DashboardSummaryDto
{
    public int TotalStudents { get; set; }

    public int TotalCompanies { get; set; }

    public int TotalJobDrives { get; set; }

    public int PendingJobDriveApprovals { get; set; }

    public int TotalApplications { get; set; }

    public int SelectedStudents { get; set; }

    public decimal PlacementPercentage { get; set; }

    public decimal AveragePackage { get; set; }

    public decimal HighestPackage { get; set; }
}
