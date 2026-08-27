using AIPlacement.Application.Admin.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AIPlacement.Application.Authentication;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = RoleNames.Admin)]
public class AdminUsersController : ControllerBase
{
    private readonly IUserRecordsService _userRecordsService;

    public AdminUsersController(IUserRecordsService userRecordsService)
    {
        _userRecordsService = userRecordsService;
    }

    [HttpGet("students")]
    public async Task<IActionResult> GetStudents()
    {
        var students = await _userRecordsService.GetStudentsAsync();
        return Ok(students);
    }

    [HttpGet("recruiters")]
    public async Task<IActionResult> GetRecruiters()
    {
        var recruiters = await _userRecordsService.GetRecruitersAsync();
        return Ok(recruiters);
    }

    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var user = await _userRecordsService.GetByUserIdAsync(userId);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPut("{userId:int}/status")]
    public async Task<IActionResult> SetActiveStatus(int userId, [FromQuery] bool isActive)
    {
        var updated = await _userRecordsService.SetActiveStatusAsync(userId, isActive);

        if (!updated)
            return NotFound();

        return NoContent();
    }
}
