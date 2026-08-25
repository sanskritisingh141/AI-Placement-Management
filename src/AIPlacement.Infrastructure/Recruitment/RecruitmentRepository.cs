using AIPlacement.Application.Recruitment.Interfaces;
using AIPlacement.Domain.Entities.Applications;
using AIPlacement.Domain.Entities.Recruitment;
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

    public async Task<ApplicationEntity?> GetApplicationByIdAsync(int applicationId)
    {
        return await _dbContext.Applications
            .FirstOrDefaultAsync(application =>
                application.ApplicationId == applicationId);
    }

    public async Task UpdateApplicationAsync(ApplicationEntity application)
    {
        _dbContext.Applications.Update(application);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddApplicationStatusHistoryAsync(
        ApplicationStatusHistory statusHistory)
    {
        await _dbContext.ApplicationStatusHistories.AddAsync(statusHistory);
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
}
