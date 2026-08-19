namespace AIPlacement.Application.Admin.DTOs;

public class JobDriveApprovalDto
{
    public int JobDriveId { get; set; }

    public int CompanyId { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public string JobTitle { get; set; } = string.Empty;

    public string? Location { get; set; }

    public decimal? MinCGPA { get; set; }

    public decimal? SalaryPackage { get; set; }

    public DateTime? ApplicationDeadline { get; set; }

    // Draft, Published, Closed
    public string Status { get; set; } = "Draft";

    // Pending, Approved, Rejected
    public string ApprovalStatus { get; set; } = "Pending";

    public string? RejectionReason { get; set; }

    public DateTime CreatedAt { get; set; }
}
