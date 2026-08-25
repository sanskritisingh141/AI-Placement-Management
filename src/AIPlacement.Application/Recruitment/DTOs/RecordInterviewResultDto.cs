namespace AIPlacement.Application.Recruitment.DTOs;

public class RecordInterviewResultDto
{
    public string Result { get; set; } = null!;
    public decimal Score { get; set; }
    public string? Remarks { get; set; }
}