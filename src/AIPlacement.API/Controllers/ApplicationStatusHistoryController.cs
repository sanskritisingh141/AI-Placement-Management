using AIPlacement.Application.Applications.Interfaces;
using AIPlacement.Domain.Entities.Applications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/applications/status-history")]
[Authorize(Roles = "Company,Admin")]
public class ApplicationStatusHistoryController : ControllerBase
{
    private readonly IApplicationStatusHistoryService _service;

    public ApplicationStatusHistoryController(
        IApplicationStatusHistoryService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Add(
        [FromBody] ApplicationStatusHistory history)
    {
        try
        {
            var result = await _service.AddAsync(history);
            return Ok(result);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpGet("{applicationId:int}")]
    public async Task<IActionResult> GetByApplicationId(
        int applicationId)
    {
        try
        {
            var result =
                await _service.GetByApplicationIdAsync(applicationId);

            return Ok(result);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }
}