using AIPlacement.Application.Placements.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/admin/placements")]
public class PlacementsController : ControllerBase
{
    private readonly IPlacementService _placementService;

    public PlacementsController(IPlacementService placementService)
    {
        _placementService = placementService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var placements = await _placementService.GetAllPlacementsAsync();

        return Ok(placements);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var placement = await _placementService.GetPlacementByIdAsync(id);

        if (placement == null)
            return NotFound();

        return Ok(placement);
    }
}