namespace AIPlacement.Domain.Entities.Recruitment;

public class InterviewRound
{
    public int RoundId { get; set; }

    public int JobDriveId { get; set; }

    public string RoundName { get; set; } = null!;

    public string? RoundType { get; set; }

    public int SequenceNo { get; set; }
}