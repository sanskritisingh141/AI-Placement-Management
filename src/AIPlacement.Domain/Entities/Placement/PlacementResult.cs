namespace AIPlacement.Domain.Entities.Placement;

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
}