namespace AIPlacement.Application.Resumes.DTOs;

public class ResumeDto
{
    public int ResumeId { get; set; }

    public int StudentId { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public DateTime? UploadedAt { get; set; }

    public int? VersionNo { get; set; }

    public bool? IsCurrent { get; set; }
}