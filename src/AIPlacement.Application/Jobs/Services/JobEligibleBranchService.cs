using AIPlacement.Application.Jobs.Interfaces;
using AIPlacement.Domain.Entities.Jobs;

namespace AIPlacement.Application.Jobs.Services;

public class JobEligibleBranchService : IJobEligibleBranchService
{
    private readonly IJobEligibleBranchRepository _repository;

    public JobEligibleBranchService(IJobEligibleBranchRepository repository) =>
        _repository = repository;

    public async Task<IEnumerable<JobEligibleBranch>> GetByJobDriveIdAsync(int jobDriveId)
    {
        if (jobDriveId <= 0) throw new ArgumentException("A valid JobDrive ID is required.");
        return await _repository.GetByJobDriveIdAsync(jobDriveId);
    }

    public async Task AddAsync(JobEligibleBranch branch)
    {
        if (branch.JobDriveId <= 0) throw new ArgumentException("A valid JobDrive ID is required.");
        if (string.IsNullOrWhiteSpace(branch.BranchName))
            throw new ArgumentException("Branch name is required.");
        branch.BranchName = branch.BranchName.Trim();
        if (branch.BranchName.Length > 100)
            throw new ArgumentException("Branch name cannot exceed 100 characters.");

        var existing = await _repository.GetByJobDriveIdAsync(branch.JobDriveId);
        if (existing.Any(item => item.BranchName.Equals(
                branch.BranchName, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("This branch is already eligible for the JobDrive.");
        await _repository.AddAsync(branch);
    }

    public async Task DeleteAsync(int jobBranchId)
    {
        if (jobBranchId <= 0) throw new ArgumentException("A valid branch association ID is required.");
        if (await _repository.GetByIdAsync(jobBranchId) is null)
            throw new InvalidOperationException("Eligible branch association not found.");
        await _repository.DeleteAsync(jobBranchId);
    }
}
