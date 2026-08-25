namespace AIPlacement.Domain.Entities.Students
{
    public class StudentSkill
    {
        public int StudentSkillId { get; set; }

        public int StudentId { get; set; }

        public int SkillId { get; set; }

        public string? ProficiencyLevel { get; set; }
    }
}