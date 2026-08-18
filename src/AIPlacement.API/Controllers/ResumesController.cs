using AIPlacement.Application.Resumes.DTOs;
using AIPlacement.Application.Resumes.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResumesController : ControllerBase
{
    private readonly IResumeService _resumeService;

    public ResumesController(IResumeService resumeService)
    {
        _resumeService = resumeService;
    }

    [HttpGet("{resumeId:int}")]
    public async Task<IActionResult> GetById(int resumeId)
    {
        var resume = await _resumeService.GetByIdAsync(resumeId);

        if (resume == null)
            return NotFound();

        return Ok(resume);
    }

    [HttpGet("student/{studentId:int}")]
    public async Task<IActionResult> GetByStudentId(int studentId)
    {
        var resumes = await _resumeService.GetByStudentIdAsync(studentId);

        return Ok(resumes);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ResumeDto resume)
    {
        var createdResume = await _resumeService.CreateAsync(resume);

        return Ok(createdResume);
    }

    [HttpPut("{resumeId:int}")]
    public async Task<IActionResult> Update(
        int resumeId,
        [FromBody] ResumeDto resume)
    {
        var updatedResume =
            await _resumeService.UpdateAsync(resumeId, resume);

        if (updatedResume == null)
            return NotFound();

        return Ok(updatedResume);
    }

    [HttpDelete("{resumeId:int}")]
    public async Task<IActionResult> Delete(int resumeId)
    {
        var deleted = await _resumeService.DeleteAsync(resumeId);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
