using AIPlacement.Application.Jobs.Interfaces;
using AIPlacement.Application.Recruitment.DTOs;
using AIPlacement.Application.Recruitment.Interfaces;
using AIPlacement.Domain.Entities.Applications;
using AIPlacement.Domain.Entities.Recruitment;

namespace AIPlacement.Application.Recruitment.Services;

public class RecruitmentService : IRecruitmentService
{
    private readonly IRecruitmentRepository _recruitmentRepository;
    private readonly IJobDriveRepository _jobDriveRepository;

    public RecruitmentService(
        IRecruitmentRepository recruitmentRepository,
        IJobDriveRepository jobDriveRepository)
    {
        _recruitmentRepository = recruitmentRepository;
        _jobDriveRepository = jobDriveRepository;
    }

    public async Task<IReadOnlyList<ApplicantDto>> GetApplicantsAsync(int jobDriveId)
    {
        var applications = await _recruitmentRepository
            .GetApplicationsByJobDriveIdAsync(jobDriveId);

        return applications.Select(MapApplicant).ToList();
    }

    public async Task<ApplicantDto?> UpdateApplicationStatusAsync(
        int applicationId,
        UpdateApplicationStatusDto request)
    {
        if (request.ChangedByUserId <= 0)
            throw new ArgumentException("A valid user ID is required.");

        if (string.IsNullOrWhiteSpace(request.Status))
            throw new ArgumentException("Application status is required.");

        var status = RecruitmentStatus.ValidStatuses.FirstOrDefault(
            value => string.Equals(
                value,
                request.Status.Trim(),
                StringComparison.OrdinalIgnoreCase));

        if (status is null)
            throw new ArgumentException("The supplied application status is invalid.");

        var application = await _recruitmentRepository
            .GetApplicationByIdAsync(applicationId);

        if (application is null)
            return null;

        if (application.CurrentStatus is RecruitmentStatus.Selected
            or RecruitmentStatus.Rejected)
        {
            throw new InvalidOperationException(
                "A selected or rejected application cannot be changed.");
        }

        application.CurrentStatus = status;
        application.RecruiterRemarks = request.Remarks?.Trim();

        var history = new ApplicationStatusHistory
        {
            ApplicationId = application.ApplicationId,
            Status = status,
            ChangedAt = DateTime.UtcNow,
            ChangedBy = request.ChangedByUserId,
            Remarks = request.Remarks?.Trim()
        };

        await _recruitmentRepository.UpdateApplicationAsync(application);
        await _recruitmentRepository.AddApplicationStatusHistoryAsync(history);

        return MapApplicant(application);
    }

    public async Task<InterviewRoundDto> CreateInterviewRoundAsync(
        CreateInterviewRoundDto request)
    {
        if (request.JobDriveId <= 0)
            throw new ArgumentException("A valid job drive ID is required.");

        if (string.IsNullOrWhiteSpace(request.RoundName))
            throw new ArgumentException("Interview round name is required.");

        if (request.SequenceNo <= 0)
            throw new ArgumentException("Round sequence number must be positive.");

        var jobDrive = await _jobDriveRepository.GetByIdAsync(request.JobDriveId);

        if (jobDrive is null)
            throw new ArgumentException("Job drive not found.");

        var interviewRound = new InterviewRound
        {
            JobDriveId = request.JobDriveId,
            RoundName = request.RoundName.Trim(),
            RoundType = request.RoundType?.Trim(),
            SequenceNo = request.SequenceNo
        };

        await _recruitmentRepository.AddInterviewRoundAsync(interviewRound);

        return MapInterviewRound(interviewRound);
    }

    public async Task<InterviewScheduleDto?> ScheduleInterviewAsync(
        ScheduleInterviewDto request)
    {
        if (request.ScheduledAt <= DateTime.UtcNow)
            throw new ArgumentException("Interview time must be in the future.");

        var application = await _recruitmentRepository
            .GetApplicationByIdAsync(request.ApplicationId);

        if (application is null)
            return null;

        if (application.CurrentStatus is RecruitmentStatus.Selected
            or RecruitmentStatus.Rejected)
        {
            throw new InvalidOperationException(
                "An interview cannot be scheduled for a finalised application.");
        }

        var interviewRound = await _recruitmentRepository
            .GetInterviewRoundByIdAsync(request.RoundId);

        if (interviewRound is null)
            throw new ArgumentException("Interview round not found.");

        if (interviewRound.JobDriveId != application.JobDriveId)
        {
            throw new ArgumentException(
                "The interview round does not belong to this application's job drive.");
        }

        var interviewSchedule = new InterviewSchedule
        {
            ApplicationId = request.ApplicationId,
            RoundId = request.RoundId,
            ScheduledAt = request.ScheduledAt,
            Location = request.Location?.Trim(),
            MeetingLink = request.MeetingLink?.Trim(),
            Status = "Scheduled"
        };

        await _recruitmentRepository.AddInterviewScheduleAsync(interviewSchedule);

        return MapInterviewSchedule(interviewSchedule);
    }

    public async Task<InterviewResultDto?> RecordInterviewResultAsync(
        int interviewId,
        RecordInterviewResultDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Result))
            throw new ArgumentException("Interview result is required.");

        if (request.Score < 0 || request.Score > 100)
            throw new ArgumentException("Interview score must be between 0 and 100.");

        var interviewSchedule = await _recruitmentRepository
            .GetInterviewScheduleByIdAsync(interviewId);

        if (interviewSchedule is null)
            return null;

        var existingResult = await _recruitmentRepository
            .GetInterviewResultByInterviewIdAsync(interviewId);

        if (existingResult is not null)
            throw new InvalidOperationException(
                "An interview result has already been recorded.");

        var interviewResult = new InterviewResult
        {
            InterviewId = interviewId,
            Result = request.Result.Trim(),
            Score = request.Score,
            Remarks = request.Remarks?.Trim(),
            EvaluatedAt = DateTime.UtcNow
        };

        interviewSchedule.Status = "Completed";

        await _recruitmentRepository.AddInterviewResultAsync(interviewResult);
        await _recruitmentRepository.UpdateInterviewScheduleAsync(interviewSchedule);

        return new InterviewResultDto
        {
            ResultId = interviewResult.ResultId,
            InterviewId = interviewResult.InterviewId,
            Result = interviewResult.Result ?? string.Empty,
            Score = interviewResult.Score,
            Remarks = interviewResult.Remarks,
            EvaluatedAt = interviewResult.EvaluatedAt
        };
    }

    private static ApplicantDto MapApplicant(
        AIPlacement.Domain.Entities.Applications.Application application)
    {
        return new ApplicantDto
        {
            ApplicationId = application.ApplicationId,
            StudentId = application.StudentId,
            JobDriveId = application.JobDriveId,
            AppliedAt = application.AppliedAt,
            CurrentStatus = application.CurrentStatus ?? RecruitmentStatus.Applied,
            RecruiterRemarks = application.RecruiterRemarks
        };
    }

    private static InterviewRoundDto MapInterviewRound(
        InterviewRound interviewRound)
    {
        return new InterviewRoundDto
        {
            RoundId = interviewRound.RoundId,
            JobDriveId = interviewRound.JobDriveId,
            RoundName = interviewRound.RoundName,
            RoundType = interviewRound.RoundType,
            SequenceNo = interviewRound.SequenceNo
        };
    }

    private static InterviewScheduleDto MapInterviewSchedule(
        InterviewSchedule interviewSchedule)
    {
        return new InterviewScheduleDto
        {
            InterviewId = interviewSchedule.InterviewId,
            ApplicationId = interviewSchedule.ApplicationId,
            RoundId = interviewSchedule.RoundId,
            ScheduledAt = interviewSchedule.ScheduledAt,
            Location = interviewSchedule.Location,
            MeetingLink = interviewSchedule.MeetingLink,
            Status = interviewSchedule.Status ?? "Scheduled"
        };
    }
}
