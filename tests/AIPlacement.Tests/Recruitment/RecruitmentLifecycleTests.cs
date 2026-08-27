using AIPlacement.Application.Jobs;
using AIPlacement.Application.Jobs.Interfaces;
using AIPlacement.Application.Recruitment;
using AIPlacement.Application.Recruitment.DTOs;
using AIPlacement.Application.Recruitment.Interfaces;
using AIPlacement.Application.Recruitment.Services;
using AIPlacement.Domain.Entities.Applications;
using AIPlacement.Domain.Entities.Jobs;
using AIPlacement.Domain.Entities.Placement;
using AIPlacement.Domain.Entities.Recruitment;
using AIPlacement.Domain.Entities.Students;
using ApplicationEntity = AIPlacement.Domain.Entities.Applications.Application;

namespace AIPlacement.Tests.Recruitment;

public class RecruitmentLifecycleTests
{
    [Fact]
    public async Task EligibilityRejectsBacklogsBranchAndRequiredSkillGaps()
    {
        var recruitment = new RecruitmentRepositoryStub
        {
            Student = new StudentProfile { StudentId=2, CGPA=8, GraduationYear=2027, CurrentBacklogs=2, Branch="ECE" },
            MissingSkills = ["SQL"]
        };
        var jobs = ValidJobRepository();
        var result = await new RecruitmentService(recruitment, jobs).CheckEligibilityAsync(2, 10);

        Assert.False(result.IsEligible);
        Assert.Contains(result.Reasons, reason => reason.Contains("backlogs"));
        Assert.Contains(result.Reasons, reason => reason.Contains("Branch"));
        Assert.Contains(result.Reasons, reason => reason.Contains("SQL"));
    }

    [Fact]
    public async Task EligibleStudentCanApplyAndStatusHistoryIsRecorded()
    {
        var repository = EligibleStudentRepository();
        var result = await new RecruitmentService(repository, ValidJobRepository())
            .ApplyAsync(new ApplyToJobDriveDto { StudentId=2, JobDriveId=10 });

        Assert.Equal(RecruitmentStatus.Applied, result.CurrentStatus);
        Assert.Single(repository.Applications);
        Assert.Single(repository.History);
    }

    [Fact]
    public async Task DuplicateApplicationIsRejected()
    {
        var repository = EligibleStudentRepository();
        repository.Exists = true;
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            new RecruitmentService(repository, ValidJobRepository())
                .ApplyAsync(new ApplyToJobDriveDto { StudentId=2, JobDriveId=10 }));
    }

    [Fact]
    public async Task SelectingApplicantCreatesPlacementWithJobPackage()
    {
        var repository = EligibleStudentRepository();
        repository.Application = new ApplicationEntity
            { ApplicationId=7, StudentId=2, JobDriveId=10, CurrentStatus=RecruitmentStatus.Shortlisted };
        await new RecruitmentService(repository, ValidJobRepository()).UpdateApplicationStatusAsync(7,
            new UpdateApplicationStatusDto { Status=RecruitmentStatus.Selected, ChangedByUserId=99, Remarks="Offer issued" });

        var placement = Assert.Single(repository.Placements);
        Assert.Equal(12.5m, placement.Package);
        Assert.Equal("Placed", placement.PlacementStatus);
    }

    [Fact]
    public async Task InterviewCanBeScheduledOnlyWithRoundFromSameJob()
    {
        var repository = EligibleStudentRepository();
        repository.Application = new ApplicationEntity
            { ApplicationId=7, StudentId=2, JobDriveId=10, CurrentStatus=RecruitmentStatus.Shortlisted };
        repository.Round = new InterviewRound { RoundId=4, JobDriveId=11, RoundName="Technical", SequenceNo=1 };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            new RecruitmentService(repository, ValidJobRepository()).ScheduleInterviewAsync(
                new ScheduleInterviewDto { ApplicationId=7, RoundId=4, ScheduledAt=DateTime.UtcNow.AddDays(1) }));
        Assert.Empty(repository.Schedules);
    }

    [Fact]
    public async Task RecordingInterviewResultCompletesScheduleAndPreventsDuplicate()
    {
        var repository = EligibleStudentRepository();
        repository.Schedule = new InterviewSchedule { InterviewId=9, ApplicationId=7, RoundId=4, Status="Scheduled" };
        var service = new RecruitmentService(repository, ValidJobRepository());
        var result = await service.RecordInterviewResultAsync(9,
            new RecordInterviewResultDto { Result="Passed", Score=88, Remarks="Strong fundamentals" });

        Assert.Equal("Completed", repository.Schedule.Status);
        Assert.Single(repository.Results);
        Assert.Equal(88, result!.Score);
        repository.ExistingResult = repository.Results[0];
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.RecordInterviewResultAsync(9,
            new RecordInterviewResultDto { Result="Passed", Score=90 }));
    }

    private static JobRepositoryStub ValidJobRepository() => new()
    {
        Job = new JobDrive { JobDriveId=10, CompanyId=3, JobTitle="Engineer", JobDescription="C# SQL",
            Location="Pune", MinCGPA=7, GraduationYear=2027, SalaryPackage=12.5m,
            ApplicationDeadline=DateTime.UtcNow.AddDays(5), Status=JobDriveStatus.Open, ApprovalStatus=JobDriveApprovalStatus.Approved },
        Criteria = new EligibilityCriteria { JobDriveId=10, MinCGPA=7, MaxBacklogs=0, GraduationYear=2027 },
        Branches = [new JobEligibleBranch { JobDriveId=10, BranchName="CSE" }]
    };

    private static RecruitmentRepositoryStub EligibleStudentRepository() => new()
    {
        Student = new StudentProfile { StudentId=2, CGPA=8, GraduationYear=2027, CurrentBacklogs=0, Branch="CSE" }
    };

    private sealed class JobRepositoryStub : IJobDriveRepository
    {
        public JobDrive? Job { get; init; }
        public EligibilityCriteria? Criteria { get; init; }
        public IReadOnlyList<JobEligibleBranch> Branches { get; init; }=[];
        public Task<JobDrive?> GetByIdAsync(int id)=>Task.FromResult(Job);
        public Task<EligibilityCriteria?> GetEligibilityCriteriaAsync(int id)=>Task.FromResult(Criteria);
        public Task<IReadOnlyList<JobEligibleBranch>> GetEligibleBranchesAsync(int id)=>Task.FromResult(Branches);
        public Task<IReadOnlyList<JobDrive>> GetAvailableAsync()=>Task.FromResult<IReadOnlyList<JobDrive>>(Job is null?[]:[Job]);
        public Task<IReadOnlyList<JobDrive>> GetByCompanyIdAsync(int id)=>GetAvailableAsync();
        public Task<IReadOnlyList<JobSkill>> GetJobSkillsAsync(int id)=>Task.FromResult<IReadOnlyList<JobSkill>>([]);
        public Task<bool> CompanyExistsAsync(int id)=>Task.FromResult(true);
        public Task<IReadOnlyList<int>> GetExistingSkillIdsAsync(IEnumerable<int> ids)=>Task.FromResult<IReadOnlyList<int>>(ids.ToList());
        public Task<IReadOnlyList<EligibilityCriteria>> GetEligibilityCriteriaBatchAsync(IEnumerable<int> ids)=>Task.FromResult<IReadOnlyList<EligibilityCriteria>>(Criteria is null?[]:[Criteria]);
        public Task<IReadOnlyList<JobSkill>> GetJobSkillsBatchAsync(IEnumerable<int> ids)=>Task.FromResult<IReadOnlyList<JobSkill>>([]);
        public Task<IReadOnlyList<JobEligibleBranch>> GetEligibleBranchesBatchAsync(IEnumerable<int> ids)=>Task.FromResult(Branches);
        public Task UpdateJobDriveAsync(JobDrive job)=>Task.CompletedTask;
        public Task AddAsync(JobDrive j,EligibilityCriteria c,IEnumerable<JobSkill>s,IEnumerable<JobEligibleBranch>b)=>Task.CompletedTask;
        public Task UpdateAsync(JobDrive j,EligibilityCriteria c,IEnumerable<JobSkill>s,IEnumerable<JobEligibleBranch>b)=>Task.CompletedTask;
    }

    private sealed class RecruitmentRepositoryStub : IRecruitmentRepository
    {
        public StudentProfile? Student { get; init; }
        public IReadOnlyList<string> MissingSkills { get; init; }=[];
        public bool Exists { get; set; }
        public ApplicationEntity? Application { get; set; }
        public List<ApplicationEntity> Applications { get; }=[];
        public List<ApplicationStatusHistory> History { get; }=[];
        public List<PlacementResult> Placements { get; }=[];
        public InterviewRound? Round { get; set; }
        public InterviewSchedule? Schedule { get; set; }
        public InterviewResult? ExistingResult { get; set; }
        public List<InterviewSchedule> Schedules { get; }=[];
        public List<InterviewResult> Results { get; }=[];
        public Task<StudentProfile?> GetStudentProfileAsync(int id)=>Task.FromResult(Student);
        public Task<IReadOnlyList<string>> GetMissingRequiredSkillsAsync(int s,int j)=>Task.FromResult(MissingSkills);
        public Task<bool> ApplicationExistsAsync(int s,int j)=>Task.FromResult(Exists);
        public Task AddApplicationAsync(ApplicationEntity a){a.ApplicationId=1;Applications.Add(a);return Task.CompletedTask;}
        public Task AddApplicationStatusHistoryAsync(ApplicationStatusHistory h){History.Add(h);return Task.CompletedTask;}
        public Task<ApplicationEntity?> GetApplicationByIdAsync(int id)=>Task.FromResult(Application);
        public Task UpdateApplicationAsync(ApplicationEntity a)=>Task.CompletedTask;
        public Task AddPlacementResultAsync(PlacementResult p){Placements.Add(p);return Task.CompletedTask;}
        public Task<decimal?> GetMatchScoreAsync(int s,int j)=>Task.FromResult<decimal?>(80);
        public Task<IReadOnlyList<ApplicationEntity>> GetApplicationsByJobDriveIdAsync(int id)=>Task.FromResult<IReadOnlyList<ApplicationEntity>>(Applications);
        public Task<IReadOnlyList<ApplicantDto>> GetApplicantDetailsByJobDriveIdAsync(int id)=>Task.FromResult<IReadOnlyList<ApplicantDto>>([]);
        public Task<IReadOnlyList<ApplicantDto>> GetApplicationsByStudentIdAsync(int id)=>Task.FromResult<IReadOnlyList<ApplicantDto>>([]);
        public Task<IReadOnlyList<(int StudentId,decimal MatchScore)>> GetMatchScoresByJobDriveAsync(int id)=>Task.FromResult<IReadOnlyList<(int,decimal)>>([]);
        public Task AddInterviewRoundAsync(InterviewRound x)=>Task.CompletedTask;
        public Task<InterviewRound?> GetInterviewRoundByIdAsync(int id)=>Task.FromResult(Round);
        public Task AddInterviewScheduleAsync(InterviewSchedule x){Schedules.Add(x);return Task.CompletedTask;}
        public Task<InterviewSchedule?> GetInterviewScheduleByIdAsync(int id)=>Task.FromResult(Schedule);
        public Task UpdateInterviewScheduleAsync(InterviewSchedule x)=>Task.CompletedTask;
        public Task AddInterviewResultAsync(InterviewResult x){Results.Add(x);return Task.CompletedTask;}
        public Task<InterviewResult?> GetInterviewResultByInterviewIdAsync(int id)=>Task.FromResult(ExistingResult);
        public Task<int?> GetCompanyIdForJobDriveAsync(int id)=>Task.FromResult<int?>(3);
        public Task<int?> GetCompanyIdForApplicationAsync(int id)=>Task.FromResult<int?>(3);
        public Task<int?> GetCompanyIdForRoundAsync(int id)=>Task.FromResult<int?>(3);
        public Task<int?> GetCompanyIdForInterviewAsync(int id)=>Task.FromResult<int?>(3);
        public Task<IReadOnlyList<InterviewRoundDto>> GetInterviewRoundsAsync(int id)=>Task.FromResult<IReadOnlyList<InterviewRoundDto>>([]);
        public Task<IReadOnlyList<InterviewScheduleDto>> GetInterviewSchedulesAsync(int id)=>Task.FromResult<IReadOnlyList<InterviewScheduleDto>>([]);
        public Task<bool> CompanyHasApplicantAsync(int companyId,int studentId)=>Task.FromResult(companyId==3&&studentId==2);
    }
}
