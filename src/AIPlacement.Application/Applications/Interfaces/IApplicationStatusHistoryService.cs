using AIPlacement.Domain.Entities.Applications;

namespace AIPlacement.Application.Applications.Interfaces;

public interface IApplicationStatusHistoryService
{
    Task<ApplicationStatusHistory> AddAsync(
        ApplicationStatusHistory history);

    Task<IReadOnlyList<ApplicationStatusHistory>>
        GetByApplicationIdAsync(int applicationId);
}