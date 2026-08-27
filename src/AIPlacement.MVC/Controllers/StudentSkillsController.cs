using AIPlacement.Application.Authentication;
using AIPlacement.Application.Skills.DTOs;
using AIPlacement.Application.Skills.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AIPlacement.MVC.Controllers;
[Authorize(Roles = RoleNames.Student)]
public class StudentSkillsController(ISkillService skills) : Controller
{
    private int StudentId => int.Parse(User.FindFirstValue("profile_id")!);
    public async Task<IActionResult> Index() => View(await skills.GetByStudentIdAsync(StudentId));
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(string skillName, string proficiencyLevel)
    {
        if (string.IsNullOrWhiteSpace(skillName)) { TempData["Error"]="Skill name is required."; return RedirectToAction(nameof(Index)); }
        await skills.CreateAsync(new SkillDto { StudentId=StudentId, SkillName=skillName.Trim(), ProficiencyLevel=proficiencyLevel });
        return RedirectToAction(nameof(Index));
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int skillId) { await skills.DeleteAsync(StudentId, skillId); return RedirectToAction(nameof(Index)); }
}
