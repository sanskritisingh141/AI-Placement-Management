using AIPlacement.Application.Jobs;
using AIPlacement.Application.Jobs.Interfaces;
using AIPlacement.Domain.Entities.Jobs;
using AIPlacement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AIPlacement.Infrastructure.Jobs;

public class JobDriveRepository : IJobDriveRepository
{
    private readonly ApplicationDbContext _dbContext;

    public JobDriveRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<JobDrive>> GetAvailableAsync()
    {
        return await _dbContext.JobDrives
            .Where(j => j.Status == JobDriveStatus.Open
                     && j.ApprovalStatus == JobDriveApprovalStatus.Approved
                     && j.ApplicationDeadline >= DateTime.UtcNow)
            .OrderBy(j => j.ApplicationDeadline)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<JobDrive>> GetByCompanyIdAsync(int companyId)
    {
        return await _dbContext.JobDrives
            .Where(j => j.CompanyId == companyId)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();
    }

    public async Task<JobDrive?> GetByIdAsync(int jobDriveId)
    {
        return await _dbContext.JobDrives
            .FirstOrDefaultAsync(j => j.JobDriveId == jobDriveId);
    }

    public async Task<EligibilityCriteria?> GetEligibilityCriteriaAsync(int jobDriveId)
    {
        return await _dbContext.EligibilityCriterias
            .FirstOrDefaultAsync(e => e.JobDriveId == jobDriveId);
    }

    public async Task<IReadOnlyList<JobSkill>> GetJobSkillsAsync(int jobDriveId)
    {
        return await _dbContext.JobSkills
            .Where(s => s.JobDriveId == jobDriveId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<JobEligibleBranch>> GetEligibleBranchesAsync(int jobDriveId)
    {
        return await _dbContext.JobEligibleBranches
            .Where(b => b.JobDriveId == jobDriveId)
            .ToListAsync();
    }

    public async Task<bool> CompanyExistsAsync(int companyId)
    {
        return await _dbContext.CompanyProfiles
            .AnyAsync(company => company.CompanyId == companyId);
    }

    public async Task<IReadOnlyList<int>> GetExistingSkillIdsAsync(
        IEnumerable<int> skillIds)
    {
        var ids = skillIds.Distinct().ToList();

        return await _dbContext.Skills
            .Where(skill => ids.Contains(skill.SkillId))
            .Select(skill => skill.SkillId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<EligibilityCriteria>> GetEligibilityCriteriaBatchAsync(
        IEnumerable<int> jobDriveIds)
    {
        var ids = jobDriveIds.ToList();
        return await _dbContext.EligibilityCriterias
            .Where(e => ids.Contains(e.JobDriveId))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<JobSkill>> GetJobSkillsBatchAsync(
        IEnumerable<int> jobDriveIds)
    {
        var ids = jobDriveIds.ToList();
        return await _dbContext.JobSkills
            .Where(s => ids.Contains(s.JobDriveId))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<JobEligibleBranch>> GetEligibleBranchesBatchAsync(
        IEnumerable<int> jobDriveIds)
    {
        var ids = jobDriveIds.ToList();
        return await _dbContext.JobEligibleBranches
            .Where(b => ids.Contains(b.JobDriveId))
            .ToListAsync();
    }

    public async Task AddAsync(
        JobDrive jobDrive,
        EligibilityCriteria eligibilityCriteria,
        IEnumerable<JobSkill> jobSkills,
        IEnumerable<JobEligibleBranch> eligibleBranches)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        await _dbContext.JobDrives.AddAsync(jobDrive);
        await _dbContext.SaveChangesAsync();

        eligibilityCriteria.JobDriveId = jobDrive.JobDriveId;

        foreach (var skill in jobSkills)
        {
            skill.JobDriveId = jobDrive.JobDriveId;
        }

        foreach (var branch in eligibleBranches)
        {
            branch.JobDriveId = jobDrive.JobDriveId;
        }

        await _dbContext.EligibilityCriterias.AddAsync(eligibilityCriteria);
        await _dbContext.JobSkills.AddRangeAsync(jobSkills);
        await _dbContext.JobEligibleBranches.AddRangeAsync(eligibleBranches);
        await _dbContext.SaveChangesAsync();

        await transaction.CommitAsync();
    }

    public async Task UpdateJobDriveAsync(JobDrive jobDrive)
    {
        _dbContext.JobDrives.Update(jobDrive);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        JobDrive jobDrive,
        EligibilityCriteria eligibilityCriteria,
        IEnumerable<JobSkill> jobSkills,
        IEnumerable<JobEligibleBranch> eligibleBranches)
    {
        _dbContext.JobDrives.Update(jobDrive);

        var existingCriteria = await _dbContext.EligibilityCriterias
            .FirstOrDefaultAsync(e => e.JobDriveId == jobDrive.JobDriveId);

        if (existingCriteria is null)
        {
            eligibilityCriteria.JobDriveId = jobDrive.JobDriveId;
            await _dbContext.EligibilityCriterias.AddAsync(eligibilityCriteria);
        }
        else
        {
            existingCriteria.MinCGPA = eligibilityCriteria.MinCGPA;
            existingCriteria.MaxBacklogs = eligibilityCriteria.MaxBacklogs;
            existingCriteria.GraduationYear = eligibilityCriteria.GraduationYear;
        }

        var existingSkills = await _dbContext.JobSkills
            .Where(s => s.JobDriveId == jobDrive.JobDriveId)
            .ToListAsync();

        var existingBranches = await _dbContext.JobEligibleBranches
            .Where(b => b.JobDriveId == jobDrive.JobDriveId)
            .ToListAsync();

        _dbContext.JobSkills.RemoveRange(existingSkills);
        _dbContext.JobEligibleBranches.RemoveRange(existingBranches);

        foreach (var skill in jobSkills)
        {
            skill.JobDriveId = jobDrive.JobDriveId;
        }

        foreach (var branch in eligibleBranches)
        {
            branch.JobDriveId = jobDrive.JobDriveId;
        }

        await _dbContext.JobSkills.AddRangeAsync(jobSkills);
        await _dbContext.JobEligibleBranches.AddRangeAsync(eligibleBranches);
        await _dbContext.SaveChangesAsync();
    }
}
