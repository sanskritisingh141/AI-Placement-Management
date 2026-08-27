using AIPlacement.Application.Authentication;
using AIPlacement.Application.Recruitment.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AIPlacement.MVC.Controllers;

[Authorize(Roles = RoleNames.Student)]
public class StudentApplicationsController(IRecruitmentService recruitment) : Controller
{
    public async Task<IActionResult> Index() => View(await recruitment.GetStudentApplicationsAsync(
        int.Parse(User.FindFirstValue("profile_id")!)));
}
