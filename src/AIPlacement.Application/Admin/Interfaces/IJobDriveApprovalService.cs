using AIPlacement.Application.Admin.DTOs;

namespace AIPlacement.Application.Admin.Interfaces;

public interface IJobDriveApprovalService
{
    Task<IReadOnlyList<JobDriveApprovalDto>> GetPendingAsync();

    Task<IReadOnlyList<JobDriveApprovalDto>> GetAllAsync();

    Task<JobDriveApprovalDto?> ApproveAsync(int jobDriveId);

    Task<JobDriveApprovalDto?> RejectAsync(int jobDriveId, string reason);
}
