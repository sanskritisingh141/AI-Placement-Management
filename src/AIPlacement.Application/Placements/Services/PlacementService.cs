using AIPlacement.Application.Admin.Services;
using AIPlacement.Application.Placements.DTOs;
using AIPlacement.Application.Placements.Interfaces;

namespace AIPlacement.Application.Placements.Services;

public class PlacementService : IPlacementService
{
    public Task<IReadOnlyList<PlacementDto>> GetAllPlacementsAsync()
    {
        var placements = AdminMockDataStore.Placements
            .Select(MapToDto)
            .ToList();

        IReadOnlyList<PlacementDto> result = placements;

        return Task.FromResult(result);
    }

    public Task<PlacementDto?> GetPlacementByIdAsync(int placementId)
    {
        var placement = AdminMockDataStore.Placements
            .FirstOrDefault(p => p.PlacementId == placementId);

        return Task.FromResult(
            placement == null ? null : MapToDto(placement));
    }

    private static PlacementDto MapToDto(
        AIPlacement.Application.Admin.DTOs.PlacementRecordDto placement)
    {
        return new PlacementDto
        {
            PlacementId = placement.PlacementId,
            StudentId = placement.StudentId,
            StudentName = placement.StudentName,
            RollNo = placement.RollNo,
            Branch = placement.Branch,
            JobDriveId = placement.JobDriveId,
            JobTitle = placement.JobTitle,
            CompanyName = placement.CompanyName,
            PlacementStatus = placement.PlacementStatus,
            Package = placement.Package,
            PlacementDate = placement.PlacementDate
        };
    }
}