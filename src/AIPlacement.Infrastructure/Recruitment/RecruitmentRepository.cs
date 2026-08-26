using AIPlacement.Application.Recruitment.Interfaces;
using AIPlacement.Domain.Entities.Applications;
using AIPlacement.Domain.Entities.Placement;
using AIPlacement.Domain.Entities.Recruitment;
using AIPlacement.Domain.Entities.Students;
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
}
