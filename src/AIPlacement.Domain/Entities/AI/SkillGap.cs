namespace AIPlacement.Domain.Entities.AI;

public class SkillGap
{
    public int SkillGapId { get; set; }

    public int MatchId { get; set; }

    public int SkillId { get; set; }

    public string? GapLevel { get; set; }

    public string? Recommendation { get; set; }
}