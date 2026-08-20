namespace AIPlacement.Domain.Entities.Recruitment;

public class InterviewResult
{
    public int ResultId { get; set; }

    public int InterviewId { get; set; }

    public string? Result { get; set; }

    public decimal Score { get; set; }

    public string? Remarks { get; set; }

    public DateTime EvaluatedAt { get; set; }
}