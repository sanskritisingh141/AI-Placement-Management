using AIPlacement.Domain.Entities.Jobs;

namespace AIPlacement.Application.Jobs.Interfaces;

public interface IJobDriveRepository
{
    Task<IReadOnlyList<JobDrive>> GetAvailableAsync();
    Task<IReadOnlyList<JobDrive>> GetByCompanyIdAsync(int companyId);
    Task<JobDrive?> GetByIdAsync(int jobDriveId);

    Task<EligibilityCriteria?> GetEligibilityCriteriaAsync(int jobDriveId);
    Task<IReadOnlyList<JobSkill>> GetJobSkillsAsync(int jobDriveId);
    Task<IReadOnlyList<JobEligibleBranch>> GetEligibleBranchesAsync(int jobDriveId);

    Task<IReadOnlyList<EligibilityCriteria>> GetEligibilityCriteriaBatchAsync(IEnumerable<int> jobDriveIds);
    Task<IReadOnlyList<JobSkill>> GetJobSkillsBatchAsync(IEnumerable<int> jobDriveIds);
    Task<IReadOnlyList<JobEligibleBranch>> GetEligibleBranchesBatchAsync(IEnumerable<int> jobDriveIds);

    Task UpdateJobDriveAsync(JobDrive jobDrive);
    Task AddAsync(
        JobDrive jobDrive,
        EligibilityCriteria eligibilityCriteria,
        IEnumerable<JobSkill> jobSkills,
        IEnumerable<JobEligibleBranch> eligibleBranches);

    Task UpdateAsync(
        JobDrive jobDrive,
        EligibilityCriteria eligibilityCriteria,
        IEnumerable<JobSkill> jobSkills,
        IEnumerable<JobEligibleBranch> eligibleBranches);
}
