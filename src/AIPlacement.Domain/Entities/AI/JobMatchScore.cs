namespace AIPlacement.Domain.Entities.AI;

public class JobMatchScore
{
    public int MatchId { get; set; }

    public int StudentId { get; set; }

    public int JobDriveId { get; set; }

    public int ResumeId { get; set; }

    public decimal MatchScore { get; set; }

    public DateTime CalculatedAt { get; set; }
}