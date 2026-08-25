using AIPlacement.Domain.Entities.Applications;
using AIPlacement.Domain.Entities.Recruitment;

using ApplicationEntity = AIPlacement.Domain.Entities.Applications.Application;

namespace AIPlacement.Application.Recruitment.Interfaces;

public interface IRecruitmentRepository
{
    Task<IReadOnlyList<ApplicationEntity>> GetApplicationsByJobDriveIdAsync(
        int jobDriveId);

    Task<ApplicationEntity?> GetApplicationByIdAsync(int applicationId);

    Task UpdateApplicationAsync(ApplicationEntity application);

    Task AddApplicationStatusHistoryAsync(
        ApplicationStatusHistory statusHistory);

    Task AddInterviewRoundAsync(InterviewRound interviewRound);

    Task<InterviewRound?> GetInterviewRoundByIdAsync(int roundId);

    Task AddInterviewScheduleAsync(InterviewSchedule interviewSchedule);

    Task<InterviewSchedule?> GetInterviewScheduleByIdAsync(int interviewId);
    Task UpdateInterviewScheduleAsync(InterviewSchedule interviewSchedule);


    Task AddInterviewResultAsync(InterviewResult interviewResult);
    Task<InterviewResult?> GetInterviewResultByInterviewIdAsync(int interviewId);
}