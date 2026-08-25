namespace AIPlacement.Application.Recruitment.DTOs;

public class UpdateApplicationStatusDto
{
    public string Status { get; set; } = null!;
    public string? Remarks { get; set; }
    public int ChangedByUserId { get; set; }
}
