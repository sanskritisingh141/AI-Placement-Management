using AIPlacement.Application.Company.DTOs;
using AIPlacement.Application.Company.Interfaces;
using AIPlacement.MVC.Models.CompanyAndJob;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.MVC.Controllers;

public class CompanyProfileController : Controller
{
    private readonly ICompanyService _companyService;

    public CompanyProfileController(ICompanyService companyService) =>
        _companyService = companyService;

    [HttpGet]
    public async Task<IActionResult> Details(int companyId)
    {
        if (companyId <= 0) return BadRequest("A valid company ID is required.");
        var company = await _companyService.GetByIdAsync(companyId);
        return company is null ? NotFound("Company profile not found.") : View(company);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int companyId)
    {
        if (companyId <= 0) return BadRequest("A valid company ID is required.");
        var company = await _companyService.GetByIdAsync(companyId);
        if (company is null) return NotFound("Company profile not found.");
        return View(ToForm(company));
    }

    [HttpPost("/CompanyProfile/Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CompanyProfileFormViewModel model)
    {
        var existing = model.CompanyId > 0
            ? await _companyService.GetByIdAsync(model.CompanyId)
            : null;
        if (existing is null) return NotFound("Company profile not found.");
        if (existing.UserId != model.UserId)
            return BadRequest("The company profile does not match its user.");
        if (!ModelState.IsValid) return View(model);

        var updated = await _companyService.UpdateAsync(model.CompanyId, new CompanyProfileDto
        {
            CompanyId = model.CompanyId,
            UserId = model.UserId,
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
}
