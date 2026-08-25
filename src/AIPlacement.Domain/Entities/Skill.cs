namespace AIPlacement.Domain.Entities
{
    public class Skill
    {
        public int SkillId { get; set; }

        public string SkillName { get; set; } = string.Empty;

        public string? Category { get; set; }
    }
}