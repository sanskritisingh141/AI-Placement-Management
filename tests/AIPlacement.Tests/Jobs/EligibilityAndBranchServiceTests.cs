using AIPlacement.Application.Jobs.Interfaces;
using AIPlacement.Application.Jobs.Services;
using AIPlacement.Domain.Entities.Jobs;

namespace AIPlacement.Tests.Jobs;

public class EligibilityAndBranchServiceTests
{
    [Fact]
    public async Task EligibilityCriteria_AddRejectsDuplicateForJobDrive()
    {
        var repository = new EligibilityRepositoryStub
        {
            ByJobDrive = new EligibilityCriteria { EligibilityId = 1, JobDriveId = 10 }
        };
        var service = new EligibilityCriteriaService(repository);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AddAsync(ValidCriteria()));
    }

    [Fact]
    public async Task EligibilityCriteria_UpdateRejectsMovingCriteriaToAnotherJobDrive()
    {
        var repository = new EligibilityRepositoryStub
        {
            ById = new EligibilityCriteria { EligibilityId = 5, JobDriveId = 10 }
        };
        var criteria = ValidCriteria();
        criteria.EligibilityId = 5;
        criteria.JobDriveId = 11;

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            new EligibilityCriteriaService(repository).UpdateAsync(criteria));
    }

    [Fact]
    public async Task EligibleBranch_AddTrimsAndRejectsDuplicateIgnoringCase()
    {
        var repository = new BranchRepositoryStub
        {
            Items = [new JobEligibleBranch { JobBranchId = 1, JobDriveId = 10, BranchName = "CSE" }]
        };
        var service = new JobEligibleBranchService(repository);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AddAsync(new JobEligibleBranch { JobDriveId = 10, BranchName = " cse " }));
    }

    private static EligibilityCriteria ValidCriteria() => new()
    {
        JobDriveId = 10,
        MinCGPA = 7,
        MaxBacklogs = 0,
        GraduationYear = DateTime.UtcNow.Year
    };

    private sealed class EligibilityRepositoryStub : IEligibilityCriteriaRepository
    {
        public EligibilityCriteria? ByJobDrive { get; init; }
        public EligibilityCriteria? ById { get; init; }
        public Task<EligibilityCriteria?> GetByJobDriveIdAsync(int id) => Task.FromResult(ByJobDrive);
        public Task<EligibilityCriteria?> GetByIdAsync(int id) => Task.FromResult(ById);
        public Task AddAsync(EligibilityCriteria item) => Task.CompletedTask;
        public Task UpdateAsync(EligibilityCriteria item) => Task.CompletedTask;
        public Task DeleteAsync(int id) => Task.CompletedTask;
    }

    private sealed class BranchRepositoryStub : IJobEligibleBranchRepository
    {
        public IReadOnlyList<JobEligibleBranch> Items { get; init; } = [];
        public Task<IEnumerable<JobEligibleBranch>> GetByJobDriveIdAsync(int id) =>
            Task.FromResult<IEnumerable<JobEligibleBranch>>(Items);
        public Task<JobEligibleBranch?> GetByIdAsync(int id) =>
            Task.FromResult(Items.FirstOrDefault(item => item.JobBranchId == id));
        public Task AddAsync(JobEligibleBranch item) => Task.CompletedTask;
        public Task DeleteAsync(int id) => Task.CompletedTask;
    }
}
