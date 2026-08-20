namespace AIPlacement.MVC.Models.CompanyAndJob
{
    public class CompanyProfiles
    {
        public int CompanyId { get; set; }

        public int UserId { get; set; }

        public string CompanyName { get; set; } = null;

        public string? Description { get; set; }

        public string? Website { get; set; }

        public string? Industry { get; set; }

        public string? ContactEmail { get; set; }

        public string? ContactPhone { get; set; }

        public User User { get; set; } = null;

        public ICollection<JobDrive> JobDrives { get; set; }
            = new List<JobDrive>();
    }
}
