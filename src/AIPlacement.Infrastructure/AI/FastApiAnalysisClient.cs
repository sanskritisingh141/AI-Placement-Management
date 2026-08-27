using System.Net.Http.Json;
using AIPlacement.Application.AI.DTOs;
using AIPlacement.Application.AI.Interfaces;

namespace AIPlacement.Infrastructure.AI;

public class FastApiAnalysisClient : IAIAnalysisClient
{
    private readonly HttpClient _httpClient;

    public FastApiAnalysisClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ResumeAnalysisResultDto> AnalyzeResumeAsync(
        byte[] pdf,
        string fileName,
        IReadOnlyList<string> knownSkills,
        CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(pdf);
        fileContent.Headers.ContentType = new("application/pdf");
        content.Add(fileContent, "file", fileName);
        content.Add(new StringContent(string.Join('|', knownSkills)), "known_skills");

        using var response = await _httpClient.PostAsync(
            "analyze-resume",
            content,
            cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ResumeAnalysisResultDto>(
                   cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("AI service returned an empty analysis response.");
    }

    public async Task<JobMatchResultDto> MatchAsync(
        JobMatchRequestDto request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync("match", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<JobMatchResultDto>(
                   cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("AI service returned an empty match response.");
    }
}
