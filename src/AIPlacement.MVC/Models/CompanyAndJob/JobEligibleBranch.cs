namespace AIPlacement.MVC.Models.CompanyAndJob;

public class JobEligibleBranch
{
    public int JobBranchId { get; set; }

    public int JobDriveId { get; set; }

    public string BranchName { get; set; } = null!;


    // JobDrives 1 : M JobEligibleBranches
    public JobDrive JobDrive { get; set; } = null!;
}
