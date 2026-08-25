using AIPlacement.Application.Recruitment.DTOs;

namespace AIPlacement.Application.Recruitment.Interfaces;

public interface IRecruitmentService
{
    Task<IReadOnlyList<ApplicantDto>> GetApplicantsAsync(int jobDriveId);

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
}