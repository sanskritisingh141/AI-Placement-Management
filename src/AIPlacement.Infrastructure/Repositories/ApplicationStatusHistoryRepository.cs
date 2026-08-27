using AIPlacement.Application.Applications.Interfaces;
using AIPlacement.Domain.Entities.Applications;
using AIPlacement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AIPlacement.Infrastructure.Repositories;

public class ApplicationStatusHistoryRepository
    : IApplicationStatusHistoryRepository
{
    private readonly ApplicationDbContext _context;

    public ApplicationStatusHistoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationStatusHistory> AddAsync(
        ApplicationStatusHistory history)
    {
        _context.ApplicationStatusHistories.Add(history);
        await _context.SaveChangesAsync();

        return history;
    }

    public async Task<IReadOnlyList<ApplicationStatusHistory>>
        GetByApplicationIdAsync(int applicationId)
    {
        return await _context.ApplicationStatusHistories
            .Where(x => x.ApplicationId == applicationId)
            .OrderBy(x => x.ChangedAt)
            .ToListAsync();
    }
}