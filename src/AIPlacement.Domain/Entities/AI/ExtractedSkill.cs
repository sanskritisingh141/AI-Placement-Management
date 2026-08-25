namespace AIPlacement.Domain.Entities.AI;

public class ExtractedSkill
{
    public int ExtractedSkillId { get; set; }

    public int AnalysisId { get; set; }

    public int SkillId { get; set; }

    public decimal ConfidenceScore { get; set; }
}