namespace AIPlacement.Application.Recruitment.DTOs;

public class CreateInterviewRoundDto
{
    public int JobDriveId { get; set; }
    public string RoundName { get; set; } = null!;
    public string? RoundType { get; set; }
    public int SequenceNo { get; set; }
}