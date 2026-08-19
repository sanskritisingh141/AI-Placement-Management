namespace AIPlacement.Application.Admin.DTOs;

public class ApplicationMonitorDto
{
    public int ApplicationId { get; set; }

    public int StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string? RollNo { get; set; }

    public string? Branch { get; set; }

    public int JobDriveId { get; set; }

    public string JobTitle { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public DateTime AppliedAt { get; set; }

    // Applied, Shortlisted, Assessment, Interview, Selected, Rejected
    public string CurrentStatus { get; set; } = "Applied";

    public string? RecruiterRemarks { get; set; }
}
