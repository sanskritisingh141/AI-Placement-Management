using AIPlacement.Application.Authentication; using AIPlacement.Application.Projects.DTOs; using AIPlacement.Application.Projects.Interfaces; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using System.Security.Claims;
namespace AIPlacement.MVC.Controllers;
[Authorize(Roles=RoleNames.Student)] public class StudentProjectsController(IProjectService service):Controller
{
 private int StudentId=>int.Parse(User.FindFirstValue("profile_id")!);
 public async Task<IActionResult> Index()=>View(await service.GetByStudentIdAsync(StudentId));
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Save(ProjectDto model){model.StudentId=StudentId; if(model.ProjectId==0) await service.CreateAsync(model); else {var old=await service.GetByIdAsync(model.ProjectId);if(old?.StudentId!=StudentId)return Forbid();await service.UpdateAsync(model.ProjectId,model);}return RedirectToAction(nameof(Index));}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Delete(int id){var old=await service.GetByIdAsync(id);if(old?.StudentId!=StudentId)return Forbid();await service.DeleteAsync(id);return RedirectToAction(nameof(Index));}
}
