using AIPlacement.Application.Authentication;
using AIPlacement.Application.Students.DTOs;
using AIPlacement.Application.Students.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AIPlacement.MVC.Controllers;

[Authorize(Roles = RoleNames.Student)]
public class StudentProfileController(IStudentService students) : Controller
{
    private int StudentId => int.Parse(User.FindFirstValue("profile_id")!);
    public async Task<IActionResult> Index() => View(await students.GetByIdAsync(StudentId));
    public async Task<IActionResult> Edit() => View(await students.GetByIdAsync(StudentId));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(StudentDto model)
    {
        model.StudentId = StudentId;
        model.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (!ModelState.IsValid) return View(model);
        await students.UpdateAsync(StudentId, model);
        TempData["Success"] = "Profile updated.";
        return RedirectToAction(nameof(Index));
    }
}
