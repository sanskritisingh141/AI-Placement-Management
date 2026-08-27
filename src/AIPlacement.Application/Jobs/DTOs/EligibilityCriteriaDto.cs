using System.ComponentModel.DataAnnotations;

namespace AIPlacement.Application.Jobs.DTOs;

public class EligibilityCriteriaDto
{
    public int EligibilityId { get; set; }
    [Range(1, int.MaxValue)] public int JobDriveId { get; set; }
    [Range(typeof(decimal), "0", "10")] public decimal MinCGPA { get; set; }
    [Range(0, int.MaxValue)] public int MaxBacklogs { get; set; }
    [Range(2000, 2100)] public int GraduationYear { get; set; }
}
