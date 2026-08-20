using AIPlacement.Domain.Entities;

namespace AIPlacement.MVC.Models.CompanyAndJob;

public class JobSkill
{
    public int JobSkillId { get; set; }

    public int JobDriveId { get; set; }

    public int SkillId { get; set; }

    public bool IsRequired { get; set; }


    public JobDrive JobDrive { get; set; } = null!;

    public Skill Skill { get; set; } = null!;
}
