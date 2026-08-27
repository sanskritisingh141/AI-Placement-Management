using AIPlacement.Application.Admin.DTOs;

namespace AIPlacement.Application.Admin.Interfaces;

public interface IAdminRepository
{
    Task<IReadOnlyList<UserRecordDto>> GetStudentsAsync();
    Task<IReadOnlyList<UserRecordDto>> GetCompaniesAsync();
    Task<UserRecordDto?> GetUserAsync(int userId);
    Task<bool> SetUserActiveAsync(int userId, bool isActive);
    Task<IReadOnlyList<JobDriveApprovalDto>> GetJobDrivesAsync(bool pendingOnly);
    Task<JobDriveApprovalDto?> SetJobDriveApprovalAsync(int jobDriveId, string approvalStatus);
    Task<IReadOnlyList<ApplicationMonitorDto>> GetApplicationsAsync(string? status = null);
    Task<IReadOnlyList<PlacementRecordDto>> GetPlacementsAsync();
}
