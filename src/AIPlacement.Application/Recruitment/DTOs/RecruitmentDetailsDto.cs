namespace AIPlacement.Application.Recruitment.DTOs;

public class InterviewRoundDto
{
    public int RoundId { get; set; }
    public int JobDriveId { get; set; }
    public string RoundName { get; set; } = null!;
    public string? RoundType { get; set; }
    public int SequenceNo { get; set; }
}

public class InterviewScheduleDto
{
    public int InterviewId { get; set; }
    public int ApplicationId { get; set; }
    public int RoundId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string? Location { get; set; }
    public string? MeetingLink { get; set; }
    public string Status { get; set; } = null!;
}

public class InterviewResultDto
{
    public int ResultId { get; set; }
    public int InterviewId { get; set; }
    public string Result { get; set; } = null!;
    public decimal Score { get; set; }
    public string? Remarks { get; set; }
    public DateTime EvaluatedAt { get; set; }
}
