using AIPlacement.Application.Recruitment.DTOs;

namespace AIPlacement.Application.Recruitment.Interfaces;

public interface IRecruitmentService
{
    Task<EligibilityResultDto> CheckEligibilityAsync(int studentId, int jobDriveId);

    Task<ApplicantDto> ApplyAsync(ApplyToJobDriveDto request);

    Task<IReadOnlyList<ApplicantDto>> GetApplicantsAsync(int jobDriveId);

    Task<IReadOnlyList<ApplicantDto>> GetStudentApplicationsAsync(int studentId);

    Task<ApplicantDto?> UpdateApplicationStatusAsync(
        int applicationId,
        UpdateApplicationStatusDto request);

    Task<InterviewRoundDto> CreateInterviewRoundAsync(
        CreateInterviewRoundDto request);

    Task<InterviewScheduleDto?> ScheduleInterviewAsync(
        ScheduleInterviewDto request);

    Task<InterviewResultDto?> RecordInterviewResultAsync(
        int interviewId,
        RecordInterviewResultDto request);
    Task<IReadOnlyList<InterviewRoundDto>> GetInterviewRoundsAsync(int jobDriveId);
    Task<IReadOnlyList<InterviewScheduleDto>> GetInterviewSchedulesAsync(int jobDriveId);
}
