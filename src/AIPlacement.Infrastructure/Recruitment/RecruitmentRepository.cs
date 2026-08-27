using AIPlacement.Application.Recruitment.Interfaces;
using AIPlacement.Domain.Entities.Applications;
using AIPlacement.Domain.Entities.Placement;
using AIPlacement.Domain.Entities.Recruitment;
using AIPlacement.Domain.Entities.Students;
using AIPlacement.Application.Recruitment.DTOs;
using AIPlacement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

using ApplicationEntity = AIPlacement.Domain.Entities.Applications.Application;

namespace AIPlacement.Infrastructure.Recruitment;

public class RecruitmentRepository : IRecruitmentRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RecruitmentRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ApplicationEntity>> GetApplicationsByJobDriveIdAsync(
        int jobDriveId)
    {
        return await _dbContext.Applications
            .Where(application => application.JobDriveId == jobDriveId)
            .OrderByDescending(application => application.AppliedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ApplicantDto>> GetApplicantDetailsByJobDriveIdAsync(int jobDriveId)
    {
        return await (
            from application in _dbContext.Applications.AsNoTracking()
            join student in _dbContext.StudentProfiles.AsNoTracking()
                on application.StudentId equals student.StudentId
            join user in _dbContext.Users.AsNoTracking() on student.UserId equals user.UserId
            join job in _dbContext.JobDrives.AsNoTracking() on application.JobDriveId equals job.JobDriveId
            join company in _dbContext.CompanyProfiles.AsNoTracking() on job.CompanyId equals company.CompanyId
            where application.JobDriveId == jobDriveId
            join match in _dbContext.JobMatchScores.AsNoTracking()
                on new { application.StudentId, application.JobDriveId }
                equals new { match.StudentId, match.JobDriveId } into matches
            from match in matches.DefaultIfEmpty()
            orderby application.AppliedAt descending
            select new ApplicantDto
            {
                ApplicationId = application.ApplicationId,
                StudentId = application.StudentId,
                JobDriveId = application.JobDriveId,
                AppliedAt = application.AppliedAt,
                CurrentStatus = application.CurrentStatus ?? "Applied",
                RecruiterRemarks = application.RecruiterRemarks,
                MatchScore = match == null ? null : match.MatchScore,
                StudentName = user.Name,
                RollNo = student.RollNo,
                Branch = student.Branch,
                JobTitle = job.JobTitle,
                CompanyName = company.CompanyName
            }).ToListAsync();
    }

    public async Task<IReadOnlyList<ApplicantDto>> GetApplicationsByStudentIdAsync(int studentId)
    {
        return await (
            from application in _dbContext.Applications.AsNoTracking()
            join student in _dbContext.StudentProfiles.AsNoTracking()
                on application.StudentId equals student.StudentId
            join user in _dbContext.Users.AsNoTracking() on student.UserId equals user.UserId
            join job in _dbContext.JobDrives.AsNoTracking() on application.JobDriveId equals job.JobDriveId
            join company in _dbContext.CompanyProfiles.AsNoTracking() on job.CompanyId equals company.CompanyId
            where application.StudentId == studentId
            join match in _dbContext.JobMatchScores.AsNoTracking()
                on new { application.StudentId, application.JobDriveId }
                equals new { match.StudentId, match.JobDriveId } into matches
            from match in matches.DefaultIfEmpty()
            orderby application.AppliedAt descending
            select new ApplicantDto
            {
                ApplicationId = application.ApplicationId,
                StudentId = application.StudentId,
                JobDriveId = application.JobDriveId,
                AppliedAt = application.AppliedAt,
                CurrentStatus = application.CurrentStatus ?? "Applied",
                RecruiterRemarks = application.RecruiterRemarks,
                MatchScore = match == null ? null : match.MatchScore,
                StudentName = user.Name,
                RollNo = student.RollNo,
                Branch = student.Branch,
                JobTitle = job.JobTitle,
                CompanyName = company.CompanyName
            }).ToListAsync();
    }

    public async Task<ApplicationEntity?> GetApplicationByIdAsync(int applicationId)
    {
        return await _dbContext.Applications
            .FirstOrDefaultAsync(application =>
                application.ApplicationId == applicationId);
    }

    public async Task AddApplicationAsync(ApplicationEntity application)
    {
        await _dbContext.Applications.AddAsync(application);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateApplicationAsync(ApplicationEntity application)
    {
        _dbContext.Applications.Update(application);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> ApplicationExistsAsync(int studentId, int jobDriveId)
    {
        return await _dbContext.Applications
            .AnyAsync(a => a.StudentId == studentId && a.JobDriveId == jobDriveId);
    }

    public async Task AddApplicationStatusHistoryAsync(
        ApplicationStatusHistory statusHistory)
    {
        await _dbContext.ApplicationStatusHistories.AddAsync(statusHistory);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<StudentProfile?> GetStudentProfileAsync(int studentId)
    {
        return await _dbContext.StudentProfiles
            .FirstOrDefaultAsync(s => s.StudentId == studentId);
    }

    public async Task<IReadOnlyList<string>> GetMissingRequiredSkillsAsync(
        int studentId,
        int jobDriveId)
    {
        var studentSkillIds = _dbContext.StudentSkills
            .Where(studentSkill => studentSkill.StudentId == studentId)
            .Select(studentSkill => studentSkill.SkillId);

        return await (
            from jobSkill in _dbContext.JobSkills.AsNoTracking()
            join skill in _dbContext.Skills.AsNoTracking()
                on jobSkill.SkillId equals skill.SkillId
            where jobSkill.JobDriveId == jobDriveId &&
                  jobSkill.IsRequired &&
                  !studentSkillIds.Contains(jobSkill.SkillId)
            select skill.SkillName).Distinct().ToListAsync();
    }

    public async Task<decimal?> GetMatchScoreAsync(int studentId, int jobDriveId)
    {
        return await _dbContext.JobMatchScores
            .Where(m => m.StudentId == studentId && m.JobDriveId == jobDriveId)
            .Select(m => (decimal?)m.MatchScore)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<(int StudentId, decimal MatchScore)>> GetMatchScoresByJobDriveAsync(
        int jobDriveId)
    {
        var scores = await _dbContext.JobMatchScores
            .Where(m => m.JobDriveId == jobDriveId)
            .Select(m => new { m.StudentId, m.MatchScore })
            .ToListAsync();

        return scores
            .Select(m => (m.StudentId, m.MatchScore))
            .ToList();
    }

    public async Task AddPlacementResultAsync(PlacementResult placementResult)
    {
        await _dbContext.PlacementResults.AddAsync(placementResult);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddInterviewRoundAsync(InterviewRound interviewRound)
    {
        await _dbContext.InterviewRounds.AddAsync(interviewRound);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<InterviewRound?> GetInterviewRoundByIdAsync(int roundId)
    {
        return await _dbContext.InterviewRounds
            .FirstOrDefaultAsync(round => round.RoundId == roundId);
    }

    public async Task AddInterviewScheduleAsync(
        InterviewSchedule interviewSchedule)
    {
        await _dbContext.InterviewSchedules.AddAsync(interviewSchedule);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<InterviewSchedule?> GetInterviewScheduleByIdAsync(
        int interviewId)
    {
        return await _dbContext.InterviewSchedules
            .FirstOrDefaultAsync(schedule =>
                schedule.InterviewId == interviewId);
    }

    public async Task UpdateInterviewScheduleAsync(
        InterviewSchedule interviewSchedule)
    {
        _dbContext.InterviewSchedules.Update(interviewSchedule);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<InterviewResult?> GetInterviewResultByInterviewIdAsync(
        int interviewId)
    {
        return await _dbContext.InterviewResults
            .FirstOrDefaultAsync(result => result.InterviewId == interviewId);
    }

    public async Task AddInterviewResultAsync(InterviewResult interviewResult)
    {
        await _dbContext.InterviewResults.AddAsync(interviewResult);
        await _dbContext.SaveChangesAsync();
    }

    public Task<int?> GetCompanyIdForJobDriveAsync(int jobDriveId) => _dbContext.JobDrives
        .Where(x => x.JobDriveId == jobDriveId).Select(x => (int?)x.CompanyId).FirstOrDefaultAsync();

    public Task<int?> GetCompanyIdForApplicationAsync(int applicationId) =>
        (from application in _dbContext.Applications
         join job in _dbContext.JobDrives on application.JobDriveId equals job.JobDriveId
         where application.ApplicationId == applicationId
         select (int?)job.CompanyId).FirstOrDefaultAsync();

    public Task<int?> GetCompanyIdForRoundAsync(int roundId) =>
        (from round in _dbContext.InterviewRounds
         join job in _dbContext.JobDrives on round.JobDriveId equals job.JobDriveId
         where round.RoundId == roundId
         select (int?)job.CompanyId).FirstOrDefaultAsync();

    public Task<int?> GetCompanyIdForInterviewAsync(int interviewId) =>
        (from interview in _dbContext.InterviewSchedules
         join application in _dbContext.Applications on interview.ApplicationId equals application.ApplicationId
         join job in _dbContext.JobDrives on application.JobDriveId equals job.JobDriveId
         where interview.InterviewId == interviewId
         select (int?)job.CompanyId).FirstOrDefaultAsync();

    public Task<IReadOnlyList<InterviewRoundDto>> GetInterviewRoundsAsync(int jobDriveId) =>
        _dbContext.InterviewRounds.AsNoTracking().Where(x => x.JobDriveId == jobDriveId)
            .OrderBy(x => x.SequenceNo).Select(x => new InterviewRoundDto
            { RoundId=x.RoundId, JobDriveId=x.JobDriveId, RoundName=x.RoundName, RoundType=x.RoundType, SequenceNo=x.SequenceNo })
            .ToListAsync().ContinueWith<IReadOnlyList<InterviewRoundDto>>(task => task.Result);

    public Task<IReadOnlyList<InterviewScheduleDto>> GetInterviewSchedulesAsync(int jobDriveId) =>
        (from schedule in _dbContext.InterviewSchedules.AsNoTracking()
         join application in _dbContext.Applications.AsNoTracking() on schedule.ApplicationId equals application.ApplicationId
         where application.JobDriveId == jobDriveId
         orderby schedule.ScheduledAt
         select new InterviewScheduleDto { InterviewId=schedule.InterviewId, ApplicationId=schedule.ApplicationId,
             RoundId=schedule.RoundId, ScheduledAt=schedule.ScheduledAt, Location=schedule.Location,
             MeetingLink=schedule.MeetingLink, Status=schedule.Status ?? "Scheduled" })
        .ToListAsync().ContinueWith<IReadOnlyList<InterviewScheduleDto>>(task => task.Result);

    public Task<bool> CompanyHasApplicantAsync(int companyId, int studentId) =>
        (from application in _dbContext.Applications.AsNoTracking()
         join job in _dbContext.JobDrives.AsNoTracking() on application.JobDriveId equals job.JobDriveId
         where job.CompanyId == companyId && application.StudentId == studentId
         select application.ApplicationId).AnyAsync();
}
