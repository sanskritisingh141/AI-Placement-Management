using AIPlacement.Domain.Entities.Applications;
using AIPlacement.Domain.Entities.Placement;
using AIPlacement.Domain.Entities.Recruitment;
using AIPlacement.Domain.Entities.Students;
using AIPlacement.Application.Recruitment.DTOs;

using ApplicationEntity = AIPlacement.Domain.Entities.Applications.Application;

namespace AIPlacement.Application.Recruitment.Interfaces;

public interface IRecruitmentRepository
{
    Task<IReadOnlyList<ApplicationEntity>> GetApplicationsByJobDriveIdAsync(
        int jobDriveId);

    Task<IReadOnlyList<ApplicantDto>> GetApplicantDetailsByJobDriveIdAsync(int jobDriveId);
    Task<IReadOnlyList<ApplicantDto>> GetApplicationsByStudentIdAsync(int studentId);

    Task<ApplicationEntity?> GetApplicationByIdAsync(int applicationId);

    Task AddApplicationAsync(ApplicationEntity application);

    Task UpdateApplicationAsync(ApplicationEntity application);

    Task<bool> ApplicationExistsAsync(int studentId, int jobDriveId);

    Task AddApplicationStatusHistoryAsync(
        ApplicationStatusHistory statusHistory);

    Task<StudentProfile?> GetStudentProfileAsync(int studentId);

    Task<IReadOnlyList<string>> GetMissingRequiredSkillsAsync(
        int studentId,
        int jobDriveId);

    Task<decimal?> GetMatchScoreAsync(int studentId, int jobDriveId);

    Task<IReadOnlyList<(int StudentId, decimal MatchScore)>> GetMatchScoresByJobDriveAsync(
        int jobDriveId);

    Task AddPlacementResultAsync(PlacementResult placementResult);

    Task AddInterviewRoundAsync(InterviewRound interviewRound);

    Task<InterviewRound?> GetInterviewRoundByIdAsync(int roundId);

    Task AddInterviewScheduleAsync(InterviewSchedule interviewSchedule);

    Task<InterviewSchedule?> GetInterviewScheduleByIdAsync(int interviewId);

    Task UpdateInterviewScheduleAsync(InterviewSchedule interviewSchedule);

    Task AddInterviewResultAsync(InterviewResult interviewResult);

    Task<InterviewResult?> GetInterviewResultByInterviewIdAsync(int interviewId);

    Task<int?> GetCompanyIdForJobDriveAsync(int jobDriveId);
    Task<int?> GetCompanyIdForApplicationAsync(int applicationId);
    Task<int?> GetCompanyIdForRoundAsync(int roundId);
    Task<int?> GetCompanyIdForInterviewAsync(int interviewId);
    Task<IReadOnlyList<InterviewRoundDto>> GetInterviewRoundsAsync(int jobDriveId);
    Task<IReadOnlyList<InterviewScheduleDto>> GetInterviewSchedulesAsync(int jobDriveId);
    Task<bool> CompanyHasApplicantAsync(int companyId, int studentId);
}
