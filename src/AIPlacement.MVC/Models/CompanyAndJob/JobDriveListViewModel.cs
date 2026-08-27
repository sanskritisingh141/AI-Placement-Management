using AIPlacement.Application.Jobs.DTOs;

namespace AIPlacement.MVC.Models.CompanyAndJob;

public class JobDriveListViewModel
{
    public int CompanyId { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public string? SelectedStatus { get; set; }

    public string? SelectedApprovalStatus { get; set; }

    public IReadOnlyList<JobDriveDto> JobDrives { get; set; } = [];
}