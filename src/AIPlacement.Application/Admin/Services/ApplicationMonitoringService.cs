using AIPlacement.Application.Admin.DTOs;
using AIPlacement.Application.Admin.Interfaces;

namespace AIPlacement.Application.Admin.Services;

public class ApplicationMonitoringService : IApplicationMonitoringService
{
    private readonly IAdminRepository _repository;

    public ApplicationMonitoringService(IAdminRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<ApplicationMonitorDto>> GetAllApplicationsAsync() =>
        _repository.GetApplicationsAsync();

    public Task<IReadOnlyList<ApplicationMonitorDto>> GetApplicationsByStatusAsync(string status) =>
        _repository.GetApplicationsAsync(status);

    public Task<IReadOnlyList<PlacementRecordDto>> GetPlacementResultsAsync() =>
        _repository.GetPlacementsAsync();
}
