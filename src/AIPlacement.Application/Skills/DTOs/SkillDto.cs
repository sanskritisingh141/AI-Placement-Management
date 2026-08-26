namespace AIPlacement.Application.Skills.DTOs;

public class SkillDto
{
    public int SkillId { get; set; }

    public int StudentId { get; set; }

    public string SkillName { get; set; } = string.Empty;

    public string ProficiencyLevel { get; set; } = string.Empty;
}