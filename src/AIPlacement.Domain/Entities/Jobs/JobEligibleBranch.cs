namespace AIPlacement.Domain.Entities.Jobs;

public class JobEligibleBranch
{
    public int JobBranchId { get; set; }

    public int JobDriveId { get; set; }

    public string BranchName { get; set; } = null!;
}