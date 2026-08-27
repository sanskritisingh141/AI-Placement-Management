using System.Text.Json.Serialization;

namespace AIPlacement.Application.AI.DTOs;

public class ExtractedSkillResultDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("confidence")]
    public decimal Confidence { get; set; }
}

public class ResumeAnalysisResultDto
{
    public int AnalysisId { get; set; }
    public int ResumeId { get; set; }

    [JsonPropertyName("extracted_text")]
    public string ExtractedText { get; set; } = string.Empty;

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

    [JsonPropertyName("skills")]
    public List<ExtractedSkillResultDto> Skills { get; set; } = new();

    [JsonPropertyName("model_version")]
    public string ModelVersion { get; set; } = string.Empty;
}

public class JobMatchRequestDto
{
    [JsonPropertyName("resume_text")]
    public string ResumeText { get; set; } = string.Empty;

    [JsonPropertyName("job_description")]
    public string JobDescription { get; set; } = string.Empty;

    [JsonPropertyName("resume_skills")]
    public List<string> ResumeSkills { get; set; } = new();

    [JsonPropertyName("required_skills")]
    public List<string> RequiredSkills { get; set; } = new();
}

public class JobMatchResultDto
{
    public int MatchId { get; set; }
    public int StudentId { get; set; }
    public int JobDriveId { get; set; }
    public int ResumeId { get; set; }

    [JsonPropertyName("match_score")]
    public decimal MatchScore { get; set; }

    [JsonPropertyName("matched_skills")]
    public List<string> MatchedSkills { get; set; } = new();

    [JsonPropertyName("missing_skills")]
    public List<string> MissingSkills { get; set; } = new();

    [JsonPropertyName("recommendation")]
    public string Recommendation { get; set; } = string.Empty;

    [JsonPropertyName("model_version")]
    public string ModelVersion { get; set; } = string.Empty;
}

public record SkillCatalogItem(int SkillId, string SkillName);

public record ResumeAnalysisSource(
    int ResumeId,
    int StudentId,
    string FileName,
    string FilePath,
    string? ExtractedText);
