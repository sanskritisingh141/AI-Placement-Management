using AIPlacement.Application.Jobs.DTOs;
using AIPlacement.Application.Skills.DTOs;

namespace AIPlacement.MVC.Models.CompanyAndJob;

public class JobDriveDetailsViewModel
{
    public int CompanyId { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public JobDriveDto JobDrive { get; set; } = new();

    public IReadOnlyList<SkillDto> RequiredSkills { get; set; } = [];
}