namespace AIPlacement.MVC.Models.ApplicationsandRecruitment;

public class ApplicationStatusHistory
{
    public int HistoryId { get; set; }

    public int ApplicationId { get; set; }

    public string? Status { get; set; }

    public DateTime ChangedAt { get; set; }

    public int ChangedBy { get; set; }

    public string? Remarks { get; set; }


    // Applications 1 : M ApplicationStatusHistory
    public Application Application { get; set; } = null!;


    // Users 1 : M ApplicationStatusHistory
    public User ChangedByUser { get; set; } = null!;
}
