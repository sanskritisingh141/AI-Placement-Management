namespace AIPlacement.MVC.Models.ApplicationsandRecruitment;

public class InterviewResult
{
    public int ResultId { get; set; }

    public int InterviewId { get; set; }

    public string? Result { get; set; }

    public decimal Score { get; set; }

    public string? Remarks { get; set; }

    public DateTime EvaluatedAt { get; set; }


    // InterviewSchedules 1 : 1 InterviewResults
    public InterviewSchedule InterviewSchedule { get; set; } = null!;
}
