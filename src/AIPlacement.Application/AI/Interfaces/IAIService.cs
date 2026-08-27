using AIPlacement.Application.AI.DTOs;

namespace AIPlacement.Application.AI.Interfaces;

public interface IAIService
{
    Task<ResumeAnalysisResultDto> AnalyzeResumeAsync(
        int resumeId,
        byte[] pdf,
        CancellationToken cancellationToken = default);
    Task<JobMatchResultDto> CalculateMatchAsync(
        int studentId,
        int jobDriveId,
        CancellationToken cancellationToken = default);
    Task<JobMatchResultDto?> GetMatchAsync(int studentId, int jobDriveId);
}
