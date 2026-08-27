using AIPlacement.Application.Jobs.Interfaces;
using AIPlacement.Domain.Entities.Jobs;

namespace AIPlacement.Application.Jobs.Services;

public class EligibilityCriteriaService : IEligibilityCriteriaService
{
    private readonly IEligibilityCriteriaRepository _repository;

    public EligibilityCriteriaService(IEligibilityCriteriaRepository repository) =>
        _repository = repository;

    public Task<EligibilityCriteria?> GetByJobDriveIdAsync(int jobDriveId)
    {
        if (jobDriveId <= 0) throw new ArgumentException("A valid JobDrive ID is required.");
        return _repository.GetByJobDriveIdAsync(jobDriveId);
    }

    public async Task AddAsync(EligibilityCriteria criteria)
    {
        Validate(criteria);
        if (await _repository.GetByJobDriveIdAsync(criteria.JobDriveId) is not null)
            throw new InvalidOperationException("EligibilityCriteria already exists for this JobDrive.");
        await _repository.AddAsync(criteria);
    }

    public async Task UpdateAsync(EligibilityCriteria criteria)
    {
        Validate(criteria);
        if (criteria.EligibilityId <= 0)
            throw new ArgumentException("A valid eligibility ID is required.");
        var existing = await _repository.GetByIdAsync(criteria.EligibilityId);
        if (existing is null) throw new InvalidOperationException("EligibilityCriteria not found.");
        if (existing.JobDriveId != criteria.JobDriveId)
            throw new InvalidOperationException("EligibilityCriteria cannot be moved to another JobDrive.");
        await _repository.UpdateAsync(criteria);
    }

    public async Task DeleteAsync(int eligibilityId)
    {
        if (eligibilityId <= 0) throw new ArgumentException("A valid eligibility ID is required.");
        if (await _repository.GetByIdAsync(eligibilityId) is null)
            throw new InvalidOperationException("EligibilityCriteria not found.");
        await _repository.DeleteAsync(eligibilityId);
    }

    private static void Validate(EligibilityCriteria criteria)
    {
        if (criteria.JobDriveId <= 0) throw new ArgumentException("A valid JobDrive ID is required.");
        if (criteria.MinCGPA is < 0 or > 10)
            throw new ArgumentException("Minimum CGPA must be between 0 and 10.");
        if (criteria.MaxBacklogs < 0)
            throw new ArgumentException("Maximum backlogs cannot be negative.");
        if (criteria.GraduationYear < DateTime.UtcNow.Year)
            throw new ArgumentException("Graduation year cannot be in the past.");
    }
}
