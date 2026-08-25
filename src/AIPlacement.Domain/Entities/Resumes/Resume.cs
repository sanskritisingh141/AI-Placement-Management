namespace AIPlacement.Domain.Entities.Resumes
{
    public class Resume
    {
        public int ResumeId { get; set; }

        public int StudentId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; }

        public int VersionNo { get; set; }

        public bool IsCurrent { get; set; }
    }
}