using AIPlacement.Application.AI.DTOs;
using AIPlacement.Application.AI.Interfaces;
using AIPlacement.Domain.Entities.AI;
using AIPlacement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AIPlacement.Infrastructure.AI;

public class AIRepository : IAIRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AIRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ResumeAnalysisSource?> GetResumeSourceAsync(int resumeId) =>
        _dbContext.Resumes
            .Where(resume => resume.ResumeId == resumeId)
            .Select(resume => new ResumeAnalysisSource(
                resume.ResumeId,
                resume.StudentId,
                resume.FileName,
                resume.FilePath,
                _dbContext.ResumeAnalyses
                    .Where(analysis => analysis.ResumeId == resume.ResumeId)
                    .OrderByDescending(analysis => analysis.AnalyzedAt)
                    .Select(analysis => analysis.ExtractedText)
                    .FirstOrDefault()))
            .SingleOrDefaultAsync();

    public Task<ResumeAnalysisSource?> GetCurrentResumeSourceAsync(int studentId) =>
        _dbContext.Resumes
            .Where(resume => resume.StudentId == studentId && resume.IsCurrent)
            .Select(resume => new ResumeAnalysisSource(
                resume.ResumeId,
                resume.StudentId,
                resume.FileName,
                resume.FilePath,
                _dbContext.ResumeAnalyses
                    .Where(analysis => analysis.ResumeId == resume.ResumeId)
                    .OrderByDescending(analysis => analysis.AnalyzedAt)
                    .Select(analysis => analysis.ExtractedText)
                    .FirstOrDefault()))
            .SingleOrDefaultAsync();

    public async Task<IReadOnlyList<SkillCatalogItem>> GetSkillCatalogAsync() =>
        await _dbContext.Skills.AsNoTracking()
            .OrderBy(skill => skill.SkillName)
            .Select(skill => new SkillCatalogItem(skill.SkillId, skill.SkillName))
            .ToListAsync();

    public async Task<IReadOnlyList<string>> GetStudentSkillNamesAsync(int studentId) =>
        await (
            from studentSkill in _dbContext.StudentSkills.AsNoTracking()
            join skill in _dbContext.Skills.AsNoTracking()
                on studentSkill.SkillId equals skill.SkillId
            where studentSkill.StudentId == studentId
            select skill.SkillName).Distinct().ToListAsync();

    public async Task<IReadOnlyList<string>> GetRequiredSkillNamesAsync(int jobDriveId) =>
        await (
            from jobSkill in _dbContext.JobSkills.AsNoTracking()
            join skill in _dbContext.Skills.AsNoTracking()
                on jobSkill.SkillId equals skill.SkillId
            where jobSkill.JobDriveId == jobDriveId && jobSkill.IsRequired
            select skill.SkillName).Distinct().ToListAsync();

    public Task<string?> GetJobDescriptionAsync(int jobDriveId) =>
        _dbContext.JobDrives
            .Where(job => job.JobDriveId == jobDriveId)
            .Select(job => job.JobDescription)
            .SingleOrDefaultAsync();

    public async Task<ResumeAnalysisResultDto> SaveAnalysisAsync(
        int resumeId,
        ResumeAnalysisResultDto result,
        IReadOnlyDictionary<string, int> skillIdsByName)
    {
        var analysis = new ResumeAnalysis
        {
            ResumeId = resumeId,
            AnalyzedAt = DateTime.UtcNow,
            ExtractedText = result.ExtractedText,
            Summary = result.Summary,
            ModelVersion = result.ModelVersion
        };
        _dbContext.ResumeAnalyses.Add(analysis);
        await _dbContext.SaveChangesAsync();

        foreach (var skill in result.Skills
                     .GroupBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                     .Select(group => group.OrderByDescending(item => item.Confidence).First()))
        {
            if (!skillIdsByName.TryGetValue(skill.Name, out var skillId))
                continue;

            _dbContext.ExtractedSkills.Add(new ExtractedSkill
            {
                AnalysisId = analysis.AnalysisId,
                SkillId = skillId,
                ConfidenceScore = skill.Confidence
            });
        }

        await _dbContext.SaveChangesAsync();
        result.AnalysisId = analysis.AnalysisId;
        result.ResumeId = resumeId;
        return result;
    }

    public async Task<JobMatchResultDto> SaveMatchAsync(
        int studentId,
        int jobDriveId,
        int resumeId,
        JobMatchResultDto result,
        IReadOnlyDictionary<string, int> skillIdsByName)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        var existing = await _dbContext.JobMatchScores
            .FirstOrDefaultAsync(match =>
                match.StudentId == studentId && match.JobDriveId == jobDriveId);

        if (existing is null)
        {
            existing = new JobMatchScore
            {
                StudentId = studentId,
                JobDriveId = jobDriveId,
                ResumeId = resumeId
            };
            _dbContext.JobMatchScores.Add(existing);
        }
        else
        {
            var gaps = _dbContext.SkillGaps.Where(gap => gap.MatchId == existing.MatchId);
            var recommendations = _dbContext.AIRecommendations
                .Where(recommendation => recommendation.MatchId == existing.MatchId);
            _dbContext.SkillGaps.RemoveRange(gaps);
            _dbContext.AIRecommendations.RemoveRange(recommendations);
        }

        existing.ResumeId = resumeId;
        existing.MatchScore = result.MatchScore;
        existing.CalculatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        foreach (var missingSkill in result.MissingSkills.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!skillIdsByName.TryGetValue(missingSkill, out var skillId))
                continue;

            _dbContext.SkillGaps.Add(new SkillGap
            {
                MatchId = existing.MatchId,
                SkillId = skillId,
                GapLevel = "Missing",
                Recommendation = $"Develop competency in {missingSkill}."
            });
        }

        _dbContext.AIRecommendations.Add(new AIRecommendation
        {
            StudentId = studentId,
            JobDriveId = jobDriveId,
            MatchId = existing.MatchId,
            RecommendationText = result.Recommendation,
            CreatedAt = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        result.MatchId = existing.MatchId;
        result.StudentId = studentId;
        result.JobDriveId = jobDriveId;
        result.ResumeId = resumeId;
        return result;
    }

    public async Task<JobMatchResultDto?> GetMatchAsync(int studentId, int jobDriveId)
    {
        var match = await _dbContext.JobMatchScores.AsNoTracking()
            .Where(item => item.StudentId == studentId && item.JobDriveId == jobDriveId)
            .OrderByDescending(item => item.CalculatedAt)
            .FirstOrDefaultAsync();
        if (match is null)
            return null;

        var missingSkills = await (
            from gap in _dbContext.SkillGaps.AsNoTracking()
            join skill in _dbContext.Skills.AsNoTracking() on gap.SkillId equals skill.SkillId
            where gap.MatchId == match.MatchId
            select skill.SkillName).ToListAsync();
        var recommendation = await _dbContext.AIRecommendations.AsNoTracking()
            .Where(item => item.MatchId == match.MatchId)
            .OrderByDescending(item => item.CreatedAt)
            .Select(item => item.RecommendationText)
            .FirstOrDefaultAsync();

        return new JobMatchResultDto
        {
            MatchId = match.MatchId,
            StudentId = match.StudentId,
            JobDriveId = match.JobDriveId,
            ResumeId = match.ResumeId,
            MatchScore = match.MatchScore,
            MissingSkills = missingSkills,
            Recommendation = recommendation ?? string.Empty
        };
    }
}
