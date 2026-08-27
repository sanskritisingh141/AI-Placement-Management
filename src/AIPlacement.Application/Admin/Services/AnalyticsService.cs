using System.Text;
using AIPlacement.Application.Admin.DTOs;
using AIPlacement.Application.Admin.Interfaces;

namespace AIPlacement.Application.Admin.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IAdminRepository _repository;

    public AnalyticsService(IAdminRepository repository)
    {
        _repository = repository;
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
    {
        var students = await _repository.GetStudentsAsync();
        var recruiters = await _repository.GetCompaniesAsync();
        var jobDrives = await _repository.GetJobDrivesAsync(false);
        var applications = await _repository.GetApplicationsAsync();
        var placements = await _repository.GetPlacementsAsync();

        var totalStudents = students.Count;
        var placedCount = placements
            .Select(p => p.StudentId)
            .Distinct()
            .Count();

        var packages = placements
            .Where(p => p.Package.HasValue)
            .Select(p => p.Package!.Value)
            .ToList();

        var summary = new DashboardSummaryDto
        {
            TotalStudents = totalStudents,
            TotalCompanies = recruiters.Count,
            TotalJobDrives = jobDrives.Count,
            PendingJobDriveApprovals = jobDrives.Count(d => d.ApprovalStatus == "Pending"),
            TotalApplications = applications.Count,
            SelectedStudents = placedCount,
            PlacementPercentage = totalStudents == 0
                ? 0m
                : Math.Round((decimal)placedCount / totalStudents * 100m, 2),
            AveragePackage = packages.Count == 0 ? 0m : Math.Round(packages.Average(), 2),
            HighestPackage = packages.Count == 0 ? 0m : packages.Max()
        };

        return summary;
    }

    public async Task<IReadOnlyList<BranchPlacementStatDto>> GetBranchPlacementStatsAsync()
    {
        var students = await _repository.GetStudentsAsync();
        var placements = await _repository.GetPlacementsAsync();

        var stats = students
            .GroupBy(s => s.Branch ?? "Unspecified")
            .Select(group =>
            {
                var total = group.Count();
                var placed = group.Count(s =>
                    placements.Any(p => p.RollNo == s.RollNo));
                var branchPackages = placements
                    .Where(p => group.Any(s => s.RollNo == p.RollNo) && p.Package.HasValue)
                    .Select(p => p.Package!.Value)
                    .ToList();

                return new BranchPlacementStatDto
                {
                    Branch = group.Key,
                    TotalStudents = total,
                    PlacedStudents = placed,
                    PlacementPercentage = total == 0 ? 0m : Math.Round((decimal)placed / total * 100m, 2),
                    AveragePackage = branchPackages.Count == 0 ? 0m : Math.Round(branchPackages.Average(), 2)
                };
            })
            .OrderByDescending(s => s.PlacementPercentage)
            .ToList();

        return stats;
    }

    public async Task<byte[]> ExportPlacementReportCsvAsync()
    {
        var builder = new StringBuilder();
        builder.AppendLine("StudentName,RollNo,Branch,Company,JobTitle,Status,Package,PlacementDate");

        foreach (var placement in await _repository.GetPlacementsAsync())
        {
            builder.AppendLine(string.Join(',',
                Escape(placement.StudentName),
                Escape(placement.RollNo),
                Escape(placement.Branch),
                Escape(placement.CompanyName),
                Escape(placement.JobTitle),
                Escape(placement.PlacementStatus),
                placement.Package?.ToString("0.00") ?? string.Empty,
                placement.PlacementDate?.ToString("yyyy-MM-dd") ?? string.Empty));
        }

        var bytes = Encoding.UTF8.GetBytes(builder.ToString());
        return bytes;
    }

    private static string Escape(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return value.Contains(',') ? $"\"{value}\"" : value;
    }
}
