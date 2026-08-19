using AIPlacement.Application.Admin.DTOs;
using AIPlacement.Application.Admin.Interfaces;

namespace AIPlacement.Application.Admin.Services;

public class UserRecordsService : IUserRecordsService
{
    public Task<IReadOnlyList<UserRecordDto>> GetStudentsAsync()
    {
        IReadOnlyList<UserRecordDto> students = AdminMockDataStore.Students.ToList();
        return Task.FromResult(students);
    }

    public Task<IReadOnlyList<UserRecordDto>> GetRecruitersAsync()
    {
        IReadOnlyList<UserRecordDto> recruiters = AdminMockDataStore.Recruiters.ToList();
        return Task.FromResult(recruiters);
    }

    public Task<UserRecordDto?> GetByUserIdAsync(int userId)
    {
        var user = AdminMockDataStore.Students.FirstOrDefault(s => s.UserId == userId)
                   ?? AdminMockDataStore.Recruiters.FirstOrDefault(r => r.UserId == userId);

        return Task.FromResult(user);
    }

    public Task<bool> SetActiveStatusAsync(int userId, bool isActive)
    {
        var exists = AdminMockDataStore.Students.Any(s => s.UserId == userId)
                     || AdminMockDataStore.Recruiters.Any(r => r.UserId == userId);

        if (!exists)
            return Task.FromResult(false);

        AdminMockDataStore.SetActiveStatus(userId, isActive);
        return Task.FromResult(true);
    }
}
