using AIPlacement.Application.AI.DTOs;

namespace AIPlacement.Application.AI.Interfaces;

public interface IAIAnalysisClient
{
    Task<ResumeAnalysisResultDto> AnalyzeResumeAsync(
        byte[] pdf,
        string fileName,
        IReadOnlyList<string> knownSkills,
        CancellationToken cancellationToken = default);

    Task<JobMatchResultDto> MatchAsync(
        JobMatchRequestDto request,
        CancellationToken cancellationToken = default);
}
