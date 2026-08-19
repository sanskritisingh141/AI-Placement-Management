using AIPlacement.Application.Admin.DTOs;
using AIPlacement.Application.Admin.Interfaces;

namespace AIPlacement.Application.Admin.Services;

public class ApplicationMonitoringService : IApplicationMonitoringService
{
    public Task<IReadOnlyList<ApplicationMonitorDto>> GetAllApplicationsAsync()
    {
        IReadOnlyList<ApplicationMonitorDto> applications = AdminMockDataStore.Applications.ToList();
        return Task.FromResult(applications);
    }

    public Task<IReadOnlyList<ApplicationMonitorDto>> GetApplicationsByStatusAsync(string status)
    {
        IReadOnlyList<ApplicationMonitorDto> applications = AdminMockDataStore.Applications
            .Where(a => string.Equals(a.CurrentStatus, status, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Task.FromResult(applications);
    }

    public Task<IReadOnlyList<PlacementRecordDto>> GetPlacementResultsAsync()
    {
        IReadOnlyList<PlacementRecordDto> placements = AdminMockDataStore.Placements.ToList();
        return Task.FromResult(placements);
    }
}
