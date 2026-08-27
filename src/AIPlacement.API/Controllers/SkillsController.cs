using AIPlacement.Application.Skills.DTOs;
using AIPlacement.Application.Skills.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AIPlacement.Application.Authentication;
using System.Security.Claims;

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

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SkillDto>>> GetAll()
    {
        var skills = await _skillService.GetAllAsync();
        return Ok(skills);
    }

    [HttpGet("{skillId:int}")]
    public async Task<ActionResult<SkillDto>> GetById(int skillId)
    {
        var skill = await _skillService.GetByIdAsync(skillId);

        if (skill == null)
            return NotFound();

        return Ok(skill);
    }

    [Authorize(Roles = RoleNames.Student)]
    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<IEnumerable<SkillDto>>> GetByStudentId(int studentId)
    {
        if (!OwnsStudent(studentId)) return Forbid();
        var skills = await _skillService.GetByStudentIdAsync(studentId);

        return Ok(skills);
    }

    [Authorize(Roles = RoleNames.Student)]
    [HttpPost]
    public async Task<ActionResult<SkillDto>> Create(SkillDto skill)
    {
        skill.StudentId = GetStudentId();
        var createdSkill = await _skillService.CreateAsync(skill);

        return Ok(createdSkill);
    }

    [Authorize(Roles = RoleNames.Student)]
    [HttpPut("{skillId}")]
    public async Task<ActionResult<SkillDto>> Update(
        int skillId,
        SkillDto skill)
    {
        skill.StudentId = GetStudentId();
        var updatedSkill = await _skillService.UpdateAsync(skillId, skill);

        if (updatedSkill == null)
            return NotFound();

        return Ok(updatedSkill);
    }

    [Authorize(Roles = RoleNames.Student)]
    [HttpDelete("{skillId}")]
    public async Task<ActionResult> Delete(int skillId)
    {
        var deleted = await _skillService.DeleteAsync(GetStudentId(), skillId);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    private int GetStudentId() => int.Parse(User.FindFirstValue("profile_id")!);
    private bool OwnsStudent(int studentId) => GetStudentId() == studentId;
}
