namespace AIPlacement.Domain.Entities.Applications;

public class Application
{
    public int ApplicationId { get; set; }

    public int StudentId { get; set; }

    public int JobDriveId { get; set; }

    public DateTime AppliedAt { get; set; }

    public string? CurrentStatus { get; set; }

    public string? RecruiterRemarks { get; set; }
}