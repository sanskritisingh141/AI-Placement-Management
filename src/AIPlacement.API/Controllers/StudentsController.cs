using AIPlacement.Application.Students.DTOs;
using AIPlacement.Application.Students.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet("{studentId:int}")]
    public async Task<IActionResult> GetById(int studentId)
    {
        var student = await _studentService.GetByIdAsync(studentId);

        if (student == null)
            return NotFound();

        return Ok(student);
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var student = await _studentService.GetByUserIdAsync(userId);

        if (student == null)
            return NotFound();

        return Ok(student);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StudentDto student)
    {
        var createdStudent = await _studentService.CreateAsync(student);

        return Ok(createdStudent);
    }

    [HttpPut("{studentId:int}")]
    public async Task<IActionResult> Update(
        int studentId,
        [FromBody] StudentDto student)
    {
        var updatedStudent =
            await _studentService.UpdateAsync(studentId, student);

        if (updatedStudent == null)
            return NotFound();

        return Ok(updatedStudent);
    }

    [HttpDelete("{studentId:int}")]
    public async Task<IActionResult> Delete(int studentId)
    {
        var deleted = await _studentService.DeleteAsync(studentId);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}