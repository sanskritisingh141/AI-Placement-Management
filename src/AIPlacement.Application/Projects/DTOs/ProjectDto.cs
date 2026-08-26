namespace AIPlacement.Application.Projects.DTOs;

public class ProjectDto
{
    public int ProjectId { get; set; }

    public int StudentId { get; set; }

    public string? ProjectTitle { get; set; }

    public string? Description { get; set; }

    public string? TechnologiesUsed { get; set; }

    public string? ProjectUrl { get; set; }

    public DateTime? CreatedAt { get; set; }
}
