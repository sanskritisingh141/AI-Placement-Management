using AIPlacement.Application.Admin.DTOs;
using AIPlacement.Application.Admin.Interfaces;
using AIPlacement.Application.Jobs;

namespace AIPlacement.Application.Admin.Services;

public class JobDriveApprovalService : IJobDriveApprovalService
{
    private readonly IAdminRepository _repository;

    public JobDriveApprovalService(IAdminRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<JobDriveApprovalDto>> GetPendingAsync() =>
        _repository.GetJobDrivesAsync(true);

    public Task<IReadOnlyList<JobDriveApprovalDto>> GetAllAsync() =>
        _repository.GetJobDrivesAsync(false);

    public Task<JobDriveApprovalDto?> ApproveAsync(int jobDriveId) =>
        _repository.SetJobDriveApprovalAsync(jobDriveId, JobDriveApprovalStatus.Approved);

    public Task<JobDriveApprovalDto?> RejectAsync(int jobDriveId, string reason) =>
        _repository.SetJobDriveApprovalAsync(jobDriveId, JobDriveApprovalStatus.Rejected);
}
