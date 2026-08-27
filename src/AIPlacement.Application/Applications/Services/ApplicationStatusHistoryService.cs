using AIPlacement.Application.Applications.Interfaces;
using AIPlacement.Domain.Entities.Applications;

namespace AIPlacement.Application.Applications.Services;

public class ApplicationStatusHistoryService
    : IApplicationStatusHistoryService
{
    private readonly IApplicationStatusHistoryRepository _repository;

    public ApplicationStatusHistoryService(
        IApplicationStatusHistoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApplicationStatusHistory> AddAsync(
        ApplicationStatusHistory history)
    {
        if (history.ApplicationId <= 0)
            throw new ArgumentException("Invalid ApplicationId.");

        if (history.ChangedBy <= 0)
            throw new ArgumentException("Invalid ChangedBy.");

        if (string.IsNullOrWhiteSpace(history.Status))
            throw new ArgumentException("Status is required.");

        history.ChangedAt = DateTime.UtcNow;

        return await _repository.AddAsync(history);
    }

    public async Task<IReadOnlyList<ApplicationStatusHistory>>
        GetByApplicationIdAsync(int applicationId)
    {
        if (applicationId <= 0)
            throw new ArgumentException("Invalid ApplicationId.");

        return await _repository.GetByApplicationIdAsync(applicationId);
    }
}