namespace AIPlacement.Domain.Entities.Applications;

public class ApplicationStatusHistory
{
    public int HistoryId { get; set; }

    public int ApplicationId { get; set; }

    public string? Status { get; set; }

    public DateTime ChangedAt { get; set; }

    public int ChangedBy { get; set; }

    public string? Remarks { get; set; }
}