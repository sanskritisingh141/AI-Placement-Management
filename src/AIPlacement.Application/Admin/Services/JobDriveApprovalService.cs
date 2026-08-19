using AIPlacement.Application.Admin.DTOs;
using AIPlacement.Application.Admin.Interfaces;

namespace AIPlacement.Application.Admin.Services;

public class JobDriveApprovalService : IJobDriveApprovalService
{
    public Task<IReadOnlyList<JobDriveApprovalDto>> GetPendingAsync()
    {
        IReadOnlyList<JobDriveApprovalDto> pending = AdminMockDataStore.JobDrives
            .Where(d => d.ApprovalStatus == "Pending")
            .ToList();

        return Task.FromResult(pending);
    }

    public Task<IReadOnlyList<JobDriveApprovalDto>> GetAllAsync()
    {
        IReadOnlyList<JobDriveApprovalDto> all = AdminMockDataStore.JobDrives.ToList();
        return Task.FromResult(all);
    }

    public Task<JobDriveApprovalDto?> ApproveAsync(int jobDriveId)
    {
        var drive = AdminMockDataStore.SetApproval(jobDriveId, "Approved", reason: null);
        return Task.FromResult(drive);
    }

    public Task<JobDriveApprovalDto?> RejectAsync(int jobDriveId, string reason)
    {
        var drive = AdminMockDataStore.SetApproval(jobDriveId, "Rejected", reason);
        return Task.FromResult(drive);
    }
}
