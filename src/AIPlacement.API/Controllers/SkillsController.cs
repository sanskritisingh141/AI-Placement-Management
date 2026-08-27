using AIPlacement.Application.Skills.DTOs;
using AIPlacement.Application.Skills.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SkillsController : ControllerBase
{
    private readonly ISkillService _skillService;

    public SkillsController(ISkillService skillService)
    {
        _skillService = skillService;
    }

    [HttpGet("{skillId}")]

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SkillDto>>> GetAll()
    {
        var skills = await _skillService.GetAllAsync();
        return Ok(skills);
    }
    public async Task<ActionResult<SkillDto>> GetById(int skillId)
    {
        var skill = await _skillService.GetByIdAsync(skillId);

        if (skill == null)
            return NotFound();

        return Ok(skill);
    }

    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<IEnumerable<SkillDto>>> GetByStudentId(int studentId)
    {
        var skills = await _skillService.GetByStudentIdAsync(studentId);

        return Ok(skills);
    }

    [HttpPost]
    public async Task<ActionResult<SkillDto>> Create(SkillDto skill)
    {
        var createdSkill = await _skillService.CreateAsync(skill);

        return Ok(createdSkill);
    }

    [HttpPut("{skillId}")]
    public async Task<ActionResult<SkillDto>> Update(
        int skillId,
        SkillDto skill)
    {
        var updatedSkill = await _skillService.UpdateAsync(skillId, skill);

        if (updatedSkill == null)
            return NotFound();

        return Ok(updatedSkill);
    }

    [HttpDelete("{skillId}")]
    public async Task<ActionResult> Delete(int skillId)
    {
        var deleted = await _skillService.DeleteAsync(skillId);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}