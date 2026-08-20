using AIPlacement.Application.Placements.DTOs;

namespace AIPlacement.Application.Placements.Interfaces;

public interface IPlacementService
{
    Task<IReadOnlyList<PlacementDto>> GetAllPlacementsAsync();

    Task<PlacementDto?> GetPlacementByIdAsync(int placementId);
}