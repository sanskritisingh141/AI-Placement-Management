namespace AIPlacement.Application.Recruitment.DTOs;

public class EligibilityResultDto
{
    public bool IsEligible { get; set; }
    public List<string> Reasons { get; set; } = new();
}
