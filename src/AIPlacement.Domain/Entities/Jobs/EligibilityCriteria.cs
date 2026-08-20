namespace AIPlacement.Domain.Entities.Jobs;

public class EligibilityCriteria
{
    public int EligibilityId { get; set; }

    public int JobDriveId { get; set; }

    public decimal MinCGPA { get; set; }

    public int MaxBacklogs { get; set; }

    public int GraduationYear { get; set; }
}