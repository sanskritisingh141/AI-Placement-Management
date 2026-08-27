using AIPlacement.Application.Students.DTOs;
using AIPlacement.Application.Students.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AIPlacement.Application.Authentication;
using System.Security.Claims;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
        if (!CanAccess(studentId)) return Forbid();
        var student = await _studentService.GetByIdAsync(studentId);

        if (student == null)
            return NotFound();

        return Ok(student);
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        if (!User.IsInRole(RoleNames.Admin) && userId != UserId) return Forbid();
        var student = await _studentService.GetByUserIdAsync(userId);

        if (student == null)
            return NotFound();

        return Ok(student);
    }

    [Authorize(Roles = RoleNames.Admin)]
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
        if (!CanAccess(studentId)) return Forbid();
        student.StudentId = studentId;
        student.UserId = UserId;
        var updatedStudent =
            await _studentService.UpdateAsync(studentId, student);

        if (updatedStudent == null)
            return NotFound();

        return Ok(updatedStudent);
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpDelete("{studentId:int}")]
    public async Task<IActionResult> Delete(int studentId)
    {
        var deleted = await _studentService.DeleteAsync(studentId);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private bool CanAccess(int id) => User.IsInRole(RoleNames.Admin) ||
        (User.IsInRole(RoleNames.Student) && int.Parse(User.FindFirstValue("profile_id")!) == id);
}
