using AIPlacement.Application.AI.DTOs;
using AIPlacement.Application.AI.Interfaces;

namespace AIPlacement.Application.AI.Services;

public class AIService : IAIService
{
    private readonly IAIRepository _repository;
    private readonly IAIAnalysisClient _client;

    public AIService(IAIRepository repository, IAIAnalysisClient client)
    {
        _repository = repository;
        _client = client;
    }

    public async Task<ResumeAnalysisResultDto> AnalyzeResumeAsync(
        int resumeId,
        byte[] pdf,
        CancellationToken cancellationToken = default)
    {
        var resume = await _repository.GetResumeSourceAsync(resumeId)
            ?? throw new ArgumentException("Resume not found.");
        var catalog = await _repository.GetSkillCatalogAsync();
        var result = await _client.AnalyzeResumeAsync(
            pdf,
            resume.FileName,
            catalog.Select(skill => skill.SkillName).ToList(),
            cancellationToken);

        return await _repository.SaveAnalysisAsync(
            resumeId,
            result,
            CatalogByName(catalog));
    }

    public async Task<JobMatchResultDto> CalculateMatchAsync(
        int studentId,
        int jobDriveId,
        CancellationToken cancellationToken = default)
    {
        var resume = await _repository.GetCurrentResumeSourceAsync(studentId)
            ?? throw new InvalidOperationException("Upload and analyze a current resume first.");
        if (string.IsNullOrWhiteSpace(resume.ExtractedText))
            throw new InvalidOperationException("The current resume has not been analyzed.");

        var jobDescription = await _repository.GetJobDescriptionAsync(jobDriveId)
            ?? throw new ArgumentException("Job drive not found.");
        var studentSkills = await _repository.GetStudentSkillNamesAsync(studentId);
        var requiredSkills = await _repository.GetRequiredSkillNamesAsync(jobDriveId);
        var result = await _client.MatchAsync(new JobMatchRequestDto
        {
            ResumeText = resume.ExtractedText,
            JobDescription = jobDescription,
            ResumeSkills = studentSkills.ToList(),
            RequiredSkills = requiredSkills.ToList()
        }, cancellationToken);

        var catalog = await _repository.GetSkillCatalogAsync();
        return await _repository.SaveMatchAsync(
            studentId,
            jobDriveId,
            resume.ResumeId,
            result,
            CatalogByName(catalog));
    }

    public Task<JobMatchResultDto?> GetMatchAsync(int studentId, int jobDriveId) =>
        _repository.GetMatchAsync(studentId, jobDriveId);

    private static IReadOnlyDictionary<string, int> CatalogByName(
        IEnumerable<SkillCatalogItem> catalog) =>
        catalog.ToDictionary(
            skill => skill.SkillName,
            skill => skill.SkillId,
            StringComparer.OrdinalIgnoreCase);
}
