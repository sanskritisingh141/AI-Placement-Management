using AIPlacement.Application.Admin.Interfaces;
using AIPlacement.Application.Placements.DTOs;
using AIPlacement.Application.Placements.Interfaces;

namespace AIPlacement.Application.Placements.Services;

public class PlacementService : IPlacementService
{
    private readonly IAdminRepository _repository;

    public PlacementService(IAdminRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<PlacementDto>> GetAllPlacementsAsync()
    {
        return (await _repository.GetPlacementsAsync())
            .Select(MapToDto)
            .ToList();
    }

    public async Task<PlacementDto?> GetPlacementByIdAsync(int placementId)
    {
        var placement = (await _repository.GetPlacementsAsync())
            .FirstOrDefault(item => item.PlacementId == placementId);

        return placement is null ? null : MapToDto(placement);
    }

    private static PlacementDto MapToDto(
        AIPlacement.Application.Admin.DTOs.PlacementRecordDto placement)
    {
        return new PlacementDto
        {
            PlacementId = placement.PlacementId,
            StudentId = placement.StudentId,
            ApplicationId = placement.ApplicationId,
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
