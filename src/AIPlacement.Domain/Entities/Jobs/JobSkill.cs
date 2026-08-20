namespace AIPlacement.Domain.Entities.Jobs;

public class JobSkill
{
    public int JobSkillId { get; set; }

    public int JobDriveId { get; set; }

    public int SkillId { get; set; }

    public bool IsRequired { get; set; }
}