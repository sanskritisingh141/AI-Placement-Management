namespace AIPlacement.Application.Jobs.DTOs;

public class CreateJobDriveDto
{
    public int CompanyId { get; set; }
    public string JobTitle { get; set; } = null!;
    public string JobDescription { get; set; } = null!;
    public string Location { get; set; } = null!;
    public decimal MinCGPA { get; set; }
    public int MaxBacklogs { get; set; }
    public int GraduationYear { get; set; }
    public decimal SalaryPackage { get; set; }
    public DateTime ApplicationDeadline { get; set; }
    public List<int> RequiredSkillIds { get; set; } = [];
    public List<string> EligibleBranches { get; set; } = [];
}