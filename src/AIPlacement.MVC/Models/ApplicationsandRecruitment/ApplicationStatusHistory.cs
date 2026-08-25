using AIPlacement.Domain.Entities;

namespace AIPlacement.MVC.Models.ApplicationsandRecruitment
{
    public class ApplicationStatusHistory
    {
        public string? Status { get; set; }

        public DateTime ChangedAt { get; set; }

        public int ChangedBy { get; set; }

        public string? Remarks { get; set; }

        public Application Application { get; set; } = null!;

        public User ChangedByUser { get; set; } = null!;
    }
}