namespace AIPlacement.Domain.Entities.Students
{
    public class Project
    {
        public int ProjectId { get; set; }

        public int StudentId { get; set; }

        public string ProjectTitle { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? TechnologiesUsed { get; set; }

        public string? ProjectUrl { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}