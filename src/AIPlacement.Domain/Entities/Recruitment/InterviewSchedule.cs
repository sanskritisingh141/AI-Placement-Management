namespace AIPlacement.Domain.Entities.Recruitment;

public class InterviewSchedule
{
    public int InterviewId { get; set; }

    public int ApplicationId { get; set; }

    public int RoundId { get; set; }

    public DateTime ScheduledAt { get; set; }

    public string? Location { get; set; }

    public string? MeetingLink { get; set; }

    public string? Status { get; set; }
}