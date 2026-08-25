namespace AIPlacement.Application.Recruitment.DTOs;

public class ScheduleInterviewDto
{
    public int ApplicationId { get; set; }
    public int RoundId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string? Location { get; set; }
    public string? MeetingLink { get; set; }
}