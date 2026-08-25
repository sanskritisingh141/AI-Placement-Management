namespace AIPlacement.Application.Recruitment.DTOs;

public class ApplicantDto
{
    public int ApplicationId { get; set; }
    public int StudentId { get; set; }
    public int JobDriveId { get; set; }
    public DateTime AppliedAt { get; set; }
    public string CurrentStatus { get; set; } = null!;
    public string? RecruiterRemarks { get; set; }
}