using AIPlacement.Application.Company.DTOs;
using AIPlacement.Application.Company.Interfaces;
using AIPlacement.MVC.Models.CompanyAndJob;
using AIPlacement.Application.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AIPlacement.MVC.Controllers;

[Authorize(Roles = RoleNames.Company)]
public class CompanyProfileController : Controller
{
    private readonly ICompanyService _companyService;

    public CompanyProfileController(ICompanyService companyService) =>
        _companyService = companyService;

    [HttpGet]
    public async Task<IActionResult> Details()
    {
        if (!TryGetCompanyId(out var companyId)) return Forbid();
        var company = await _companyService.GetByIdAsync(companyId);
        return company is null ? NotFound("Company profile not found.") : View(company);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        if (!TryGetCompanyId(out var companyId)) return Forbid();
        var company = await _companyService.GetByIdAsync(companyId);
        if (company is null) return NotFound("Company profile not found.");
        return View(ToForm(company));
    }

    [HttpPost("/CompanyProfile/Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CompanyProfileFormViewModel model)
    {
        if (!TryGetCompanyId(out var companyId) || companyId != model.CompanyId)
            return Forbid();
        var existing = await _companyService.GetByIdAsync(companyId);
        if (existing is null) return NotFound("Company profile not found.");
        if (!ModelState.IsValid) return View(model);

        var updated = await _companyService.UpdateAsync(model.CompanyId, new CompanyProfileDto
        {
            CompanyId = model.CompanyId,
            UserId = existing.UserId,
            CompanyName = model.CompanyName,
            Description = model.Description,
            Website = model.Website,
            Industry = model.Industry,
            ContactEmail = model.ContactEmail,
            ContactPhone = model.ContactPhone
        });
        if (updated is null) return NotFound();
        TempData["SuccessMessage"] = "Company profile was updated.";
        return RedirectToAction(nameof(Details), new { companyId = model.CompanyId });
    }

    private static CompanyProfileFormViewModel ToForm(CompanyProfileDto company) => new()
    {
        CompanyId = company.CompanyId,
        UserId = company.UserId,
        CompanyName = company.CompanyName,
        Description = company.Description,
        Website = company.Website,
        Industry = company.Industry,
        ContactEmail = company.ContactEmail,
        ContactPhone = company.ContactPhone
    };

    private bool TryGetCompanyId(out int companyId) =>
        int.TryParse(User.FindFirstValue("profile_id"), out companyId);
}
