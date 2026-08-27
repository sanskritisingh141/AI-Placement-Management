using AIPlacement.Application.Admin.DTOs;
using AIPlacement.Application.Admin.Interfaces;

namespace AIPlacement.Application.Admin.Services;

public class UserRecordsService : IUserRecordsService
{
    private readonly IAdminRepository _repository;

    public UserRecordsService(IAdminRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<UserRecordDto>> GetStudentsAsync() =>
        _repository.GetStudentsAsync();

    public Task<IReadOnlyList<UserRecordDto>> GetRecruitersAsync() =>
        _repository.GetCompaniesAsync();

    public Task<UserRecordDto?> GetByUserIdAsync(int userId) =>
        _repository.GetUserAsync(userId);

    public Task<bool> SetActiveStatusAsync(int userId, bool isActive) =>
        _repository.SetUserActiveAsync(userId, isActive);
}
