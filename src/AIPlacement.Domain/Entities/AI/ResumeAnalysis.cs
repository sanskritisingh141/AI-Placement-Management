namespace AIPlacement.Domain.Entities.AI;

public class ResumeAnalysis
{
    public int AnalysisId { get; set; }

    public int ResumeId { get; set; }

    public DateTime AnalyzedAt { get; set; }

    public string? ExtractedText { get; set; }

    public string? Summary { get; set; }

    public string? ModelVersion { get; set; }
}