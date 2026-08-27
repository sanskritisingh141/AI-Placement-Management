using AIPlacement.Application.AI.DTOs;

namespace AIPlacement.Application.AI.Interfaces;

public interface IAIRepository
{
    Task<ResumeAnalysisSource?> GetResumeSourceAsync(int resumeId);
    Task<ResumeAnalysisSource?> GetCurrentResumeSourceAsync(int studentId);
    Task<IReadOnlyList<SkillCatalogItem>> GetSkillCatalogAsync();
    Task<IReadOnlyList<string>> GetStudentSkillNamesAsync(int studentId);
    Task<IReadOnlyList<string>> GetRequiredSkillNamesAsync(int jobDriveId);
    Task<string?> GetJobDescriptionAsync(int jobDriveId);
    Task<ResumeAnalysisResultDto> SaveAnalysisAsync(
        int resumeId,
        ResumeAnalysisResultDto result,
        IReadOnlyDictionary<string, int> skillIdsByName);
    Task<JobMatchResultDto> SaveMatchAsync(
        int studentId,
        int jobDriveId,
        int resumeId,
        JobMatchResultDto result,
        IReadOnlyDictionary<string, int> skillIdsByName);
    Task<JobMatchResultDto?> GetMatchAsync(int studentId, int jobDriveId);
}
