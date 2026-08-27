namespace AIPlacement.MVC.Models.CompanyAndJob;

public class CompanyDashboardViewModel
{
    public int CompanyId { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public int TotalJobDrives { get; set; }

    public int DraftJobDrives { get; set; }

    public int PendingApprovalJobDrives { get; set; }

    public int OpenJobDrives { get; set; }

    public int ClosedJobDrives { get; set; }
}