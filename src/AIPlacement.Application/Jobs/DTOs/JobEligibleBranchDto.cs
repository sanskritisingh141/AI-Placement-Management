using System.ComponentModel.DataAnnotations;

namespace AIPlacement.Application.Jobs.DTOs;

public class JobEligibleBranchDto
{
    public int JobBranchId { get; set; }
    [Range(1, int.MaxValue)] public int JobDriveId { get; set; }
    [Required, StringLength(100)] public string BranchName { get; set; } = string.Empty;
}
