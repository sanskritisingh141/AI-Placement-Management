namespace AIPlacement.Application.Recruitment.DTOs;

public class ApplicantDto
{
    public int ApplicationId { get; set; }
    public int StudentId { get; set; }
    public int JobDriveId { get; set; }
    public DateTime AppliedAt { get; set; }
    public string CurrentStatus { get; set; } = null!;
    public string? RecruiterRemarks { get; set; }
    public decimal? MatchScore { get; set; }
    public string? StudentName { get; set; }
    public string? RollNo { get; set; }
    public string? Branch { get; set; }
    public string? JobTitle { get; set; }
    public string? CompanyName { get; set; }
}
