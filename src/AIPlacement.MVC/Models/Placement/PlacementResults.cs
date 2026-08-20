using AIPlacement.MVC.Models.ApplicationsandRecruitment;
using AIPlacement.MVC.Models.CompanyAndJob;

namespace AIPlacement.MVC.Models.Placement;

public class PlacementResult
{
    public int PlacementId { get; set; }

    public int StudentId { get; set; }

    public int JobDriveId { get; set; }

    public int ApplicationId { get; set; }

    public string? PlacementStatus { get; set; }

    public decimal Package { get; set; }

    public DateTime PlacementDate { get; set; }

    public string? OfferDetails { get; set; }


    // StudentProfiles 1 : M PlacementResults
    public StudentProfile StudentProfile { get; set; } = null!;


    // JobDrives 1 : M PlacementResults
    public JobDrive JobDrive { get; set; } = null!;


    // Applications 1 : 1 PlacementResults
    public Application Application { get; set; } = null!;
}
