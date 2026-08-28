using AIPlacement.Application.AI.DTOs;
using AIPlacement.Application.Resumes.DTOs;

namespace AIPlacement.MVC.Models.StudentResumes;

public class StudentResumeIndexViewModel
{
    public IReadOnlyList<ResumeDto> Resumes { get; init; } = [];

    public IReadOnlyDictionary<int, ResumeAnalysisResultDto> LatestAnalyses { get; init; } =
        new Dictionary<int, ResumeAnalysisResultDto>();
}
