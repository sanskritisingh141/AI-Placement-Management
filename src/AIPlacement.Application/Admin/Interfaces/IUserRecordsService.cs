using AIPlacement.Application.Admin.DTOs;

namespace AIPlacement.Application.Admin.Interfaces;

public interface IUserRecordsService
{
    Task<IReadOnlyList<UserRecordDto>> GetStudentsAsync();

    Task<IReadOnlyList<UserRecordDto>> GetRecruitersAsync();

    Task<UserRecordDto?> GetByUserIdAsync(int userId);

    Task<bool> SetActiveStatusAsync(int userId, bool isActive);
}
