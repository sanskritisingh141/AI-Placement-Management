using AIPlacement.Application.DTOs;
using AIPlacement.Application.Jobs.Services;
using AIPlacement.Domain.Entities.Jobs;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EligibilityCriteriaController : ControllerBase
{
    private readonly IEligibilityCriteriaService _service;

    public EligibilityCriteriaController(IEligibilityCriteriaService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] EligibilityCriteriaDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = new EligibilityCriteria
        {
            JobDriveId = dto.JobDriveId,
            MinCGPA = dto.MinCGPA,
            MaxBacklogs = dto.MaxBacklogs,
            GraduationYear = dto.GraduationYear
        };

        await _service.AddAsync(entity);
        return Ok(dto);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] EligibilityCriteriaDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = new EligibilityCriteria
        {
            EligibilityId = dto.EligibilityId,
            JobDriveId = dto.JobDriveId,
            MinCGPA = dto.MinCGPA,
            MaxBacklogs = dto.MaxBacklogs,
            GraduationYear = dto.GraduationYear
        };

        await _service.UpdateAsync(entity);
        return Ok(dto);
    }
}
