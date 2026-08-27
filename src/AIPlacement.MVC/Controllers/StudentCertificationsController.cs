using AIPlacement.Application.Authentication; using AIPlacement.Application.Certifications.DTOs; using AIPlacement.Application.Certifications.Interfaces; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using System.Security.Claims;
namespace AIPlacement.MVC.Controllers;
[Authorize(Roles=RoleNames.Student)] public class StudentCertificationsController(ICertificationService service):Controller
{
 private int StudentId=>int.Parse(User.FindFirstValue("profile_id")!);
 public async Task<IActionResult> Index()=>View(await service.GetByStudentIdAsync(StudentId));
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Save(CertificationDto model){model.StudentId=StudentId;if(model.CertificationId==0)await service.CreateAsync(model);else{var old=await service.GetByIdAsync(model.CertificationId);if(old?.StudentId!=StudentId)return Forbid();await service.UpdateAsync(model.CertificationId,model);}return RedirectToAction(nameof(Index));}
 [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Delete(int id){var old=await service.GetByIdAsync(id);if(old?.StudentId!=StudentId)return Forbid();await service.DeleteAsync(id);return RedirectToAction(nameof(Index));}
}
