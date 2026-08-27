using AIPlacement.Application.Jobs;
using AIPlacement.Application.Jobs.DTOs;
using AIPlacement.Application.Jobs.Interfaces;
using AIPlacement.Application.Jobs.Services;
using AIPlacement.Domain.Entities.Jobs;

namespace AIPlacement.Tests.Jobs;

public class JobDriveServiceTests
{
    [Fact]
    public async Task CreateAsync_RejectsUnknownCompany()
    {
        var repository = new StubJobDriveRepository
        {
            CompanyExists = false,
            ExistingSkillIds = [10]
        };

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            new JobDriveService(repository).CreateAsync(CreateRequest([10])));

        Assert.Equal("Company not found.", exception.Message);
        Assert.Null(repository.AddedJobDrive);
    }

    [Fact]
    public async Task CreateAsync_RejectsUnknownSkillIds()
    {
        var repository = new StubJobDriveRepository
        {
            CompanyExists = true,
            ExistingSkillIds = [10]
        };

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            new JobDriveService(repository).CreateAsync(CreateRequest([10, 20])));

        Assert.Contains("20", exception.Message, StringComparison.Ordinal);
        Assert.Null(repository.AddedJobDrive);
    }

    [Fact]
    public async Task CreateAsync_RejectsMissingEligibleBranches()
    {
        var repository = new StubJobDriveRepository
        {
            CompanyExists = true,
            ExistingSkillIds = [10]
        };
        var request = CreateRequest([10]);
        request.EligibleBranches = [];

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            new JobDriveService(repository).CreateAsync(request));

        Assert.Contains("eligible branch", exception.Message,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_PersistsDistinctJobSkillsUsingExistingSkillIds()
    {
        var repository = new StubJobDriveRepository
        {
            CompanyExists = true,
            ExistingSkillIds = [10, 20]
        };

        var result = await new JobDriveService(repository)
            .CreateAsync(CreateRequest([10, 10, 20]));

        Assert.NotNull(repository.AddedJobDrive);
        Assert.Equal(JobDriveStatus.Draft, repository.AddedJobDrive.Status);
        Assert.Equal(JobDriveApprovalStatus.Pending, repository.AddedJobDrive.ApprovalStatus);
        Assert.Equal([10, 20], repository.AddedJobSkills.Select(skill => skill.SkillId));
        Assert.All(repository.AddedJobSkills, skill => Assert.True(skill.IsRequired));
        Assert.Equal([10, 20], result.RequiredSkillIds);
    }

    [Fact]
    public async Task PublishAsync_RejectsClosedJobDrive()
    {
        var repository = new StubJobDriveRepository
        {
            JobDrive = ExistingJobDrive(JobDriveStatus.Closed)
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            new JobDriveService(repository).PublishAsync(100));

        Assert.Contains("closed", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(0, repository.UpdateJobDriveCount);
    }

    [Fact]
    public async Task PublishAsync_RejectsExpiredJobDrive()
    {
        var jobDrive = ExistingJobDrive(JobDriveStatus.Draft);
        jobDrive.ApplicationDeadline = DateTime.UtcNow.AddMinutes(-1);
        var repository = new StubJobDriveRepository { JobDrive = jobDrive };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            new JobDriveService(repository).PublishAsync(100));

        Assert.Contains("expired", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(0, repository.UpdateJobDriveCount);
    }

    [Fact]
    public async Task CloseAsync_RejectsJobDriveThatIsNotOpen()
    {
        var repository = new StubJobDriveRepository
        {
            JobDrive = ExistingJobDrive(JobDriveStatus.Draft)
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            new JobDriveService(repository).CloseAsync(100));

        Assert.Equal(0, repository.UpdateJobDriveCount);
    }

    private static CreateJobDriveDto CreateRequest(List<int> skillIds) => new()
    {
        CompanyId = 1,
        JobTitle = "Software Engineer",
        JobDescription = "Build placement management features.",
        Location = "Bengaluru",
        MinCGPA = 7m,
        MaxBacklogs = 0,
        GraduationYear = DateTime.UtcNow.Year,
        SalaryPackage = 600000m,
        ApplicationDeadline = DateTime.UtcNow.AddDays(10),
        RequiredSkillIds = skillIds,
        EligibleBranches = ["CSE"]
    };

    private static JobDrive ExistingJobDrive(string status) => new()
    {
        JobDriveId = 100,
        CompanyId = 1,
        JobTitle = "Software Engineer",
        JobDescription = "Build placement management features.",
        Location = "Bengaluru",
        MinCGPA = 7m,
        GraduationYear = DateTime.UtcNow.Year,
        SalaryPackage = 600000m,
        ApplicationDeadline = DateTime.UtcNow.AddDays(10),
        Status = status,
        ApprovalStatus = JobDriveApprovalStatus.Approved,
        CreatedAt = DateTime.UtcNow
    };

    private sealed class StubJobDriveRepository : IJobDriveRepository
    {
        public bool CompanyExists { get; init; }
        public IReadOnlyList<int> ExistingSkillIds { get; init; } = [];
        public JobDrive? JobDrive { get; init; }
        public JobDrive? AddedJobDrive { get; private set; }
        public IReadOnlyList<JobSkill> AddedJobSkills { get; private set; } = [];
        public int UpdateJobDriveCount { get; private set; }

        public Task<bool> CompanyExistsAsync(int companyId) =>
            Task.FromResult(CompanyExists);

        public Task<IReadOnlyList<int>> GetExistingSkillIdsAsync(IEnumerable<int> skillIds) =>
            Task.FromResult(ExistingSkillIds);

        public Task<JobDrive?> GetByIdAsync(int jobDriveId) =>
            Task.FromResult(JobDrive?.JobDriveId == jobDriveId ? JobDrive : null);

        public Task AddAsync(
            JobDrive jobDrive,
            EligibilityCriteria eligibilityCriteria,
            IEnumerable<JobSkill> jobSkills,
            IEnumerable<JobEligibleBranch> eligibleBranches)
        {
            jobDrive.JobDriveId = 100;
            AddedJobDrive = jobDrive;
            AddedJobSkills = jobSkills.ToList();
            return Task.CompletedTask;
        }

        public Task UpdateJobDriveAsync(JobDrive jobDrive)
        {
            UpdateJobDriveCount++;
            return Task.CompletedTask;
        }

        public Task<EligibilityCriteria?> GetEligibilityCriteriaAsync(int jobDriveId) =>
            Task.FromResult<EligibilityCriteria?>(null);
        public Task<IReadOnlyList<JobSkill>> GetJobSkillsAsync(int jobDriveId) =>
            Task.FromResult<IReadOnlyList<JobSkill>>([]);
        public Task<IReadOnlyList<JobEligibleBranch>> GetEligibleBranchesAsync(int jobDriveId) =>
            Task.FromResult<IReadOnlyList<JobEligibleBranch>>([]);
        public Task<IReadOnlyList<JobDrive>> GetAvailableAsync() => throw new NotSupportedException();
        public Task<IReadOnlyList<JobDrive>> GetByCompanyIdAsync(int companyId) => throw new NotSupportedException();
        public Task<IReadOnlyList<EligibilityCriteria>> GetEligibilityCriteriaBatchAsync(IEnumerable<int> jobDriveIds) =>
            throw new NotSupportedException();
        public Task<IReadOnlyList<JobSkill>> GetJobSkillsBatchAsync(IEnumerable<int> jobDriveIds) =>
            throw new NotSupportedException();
        public Task<IReadOnlyList<JobEligibleBranch>> GetEligibleBranchesBatchAsync(IEnumerable<int> jobDriveIds) =>
            throw new NotSupportedException();
        public Task UpdateAsync(JobDrive jobDrive, EligibilityCriteria eligibilityCriteria,
            IEnumerable<JobSkill> jobSkills, IEnumerable<JobEligibleBranch> eligibleBranches) =>
            throw new NotSupportedException();
    }
}
