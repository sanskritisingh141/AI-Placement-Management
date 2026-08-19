using AIPlacement.Application.Admin.DTOs;

namespace AIPlacement.Application.Admin.Interfaces;

public interface IAnalyticsService
{
    Task<DashboardSummaryDto> GetDashboardSummaryAsync();

    Task<IReadOnlyList<BranchPlacementStatDto>> GetBranchPlacementStatsAsync();

    /// <summary>
    /// Builds a CSV placement report (TSK-25 downloadable report).
    /// </summary>
    Task<byte[]> ExportPlacementReportCsvAsync();
}
