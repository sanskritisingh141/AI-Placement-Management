using AIPlacement.Application.Admin.DTOs;

namespace AIPlacement.Application.Admin.Interfaces;

public interface IApplicationMonitoringService
{
    Task<IReadOnlyList<ApplicationMonitorDto>> GetAllApplicationsAsync();

    Task<IReadOnlyList<ApplicationMonitorDto>> GetApplicationsByStatusAsync(string status);

    Task<IReadOnlyList<PlacementRecordDto>> GetPlacementResultsAsync();
}
