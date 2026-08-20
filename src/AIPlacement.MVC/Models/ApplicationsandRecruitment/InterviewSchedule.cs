using AllPlacement.MVC.Models;

namespace AIPlacement.MVC.Models.ApplicationsandRecruitment;

public class InterviewSchedule
{
    public int InterviewId { get; set; }

    public int ApplicationId { get; set; }

    public int RoundId { get; set; }

    public DateTime ScheduledAt { get; set; }

    public string? Location { get; set; }

    public string? MeetingLink { get; set; }

    public string? Status { get; set; }


    // Applications 1 : M InterviewSchedules
    public Application Application { get; set; } = null!;


    // InterviewRounds 1 : M InterviewSchedules
    public InterviewRound InterviewRound { get; set; } = null!;


    // InterviewSchedules 1 : 1 InterviewResults
    public InterviewResult? InterviewResult { get; set; }
}
