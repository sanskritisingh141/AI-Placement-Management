using AIPlacement.Application.Company.DTOs;
using AIPlacement.Application.Company.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AIPlacement.Application.Authentication;
using System.Security.Claims;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompaniesController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpGet("{companyId:int}")]
    public async Task<IActionResult> GetById(int companyId)
    {
        if (!CanAccess(companyId)) return Forbid();
        var company = await _companyService.GetByIdAsync(companyId);

        if (company == null)
            return NotFound();

        return Ok(company);
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        if (!User.IsInRole(RoleNames.Admin) && userId != UserId) return Forbid();
        var company = await _companyService.GetByUserIdAsync(userId);

        if (company == null)
            return NotFound();

        return Ok(company);
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CompanyProfileDto company)
    {
        try
        {
            var createdCompany = await _companyService.CreateAsync(company);
            return CreatedAtAction(nameof(GetById),
                new { companyId = createdCompany.CompanyId }, createdCompany);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPut("{companyId:int}")]
    public async Task<IActionResult> Update(
        int companyId,
        [FromBody] CompanyProfileDto company)
    {
        if (!CanAccess(companyId)) return Forbid();
        company.CompanyId = companyId;
        if (!User.IsInRole(RoleNames.Admin)) company.UserId = UserId;
        try
        {
            var updatedCompany = await _companyService.UpdateAsync(companyId, company);
            return updatedCompany is null ? NotFound() : Ok(updatedCompany);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpDelete("{companyId:int}")]
    public async Task<IActionResult> Delete(int companyId)
    {
        try
        {
            var deleted = await _companyService.DeleteAsync(companyId);
            return deleted ? NoContent() : NotFound();
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = exception.Message });
        }
    }
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private bool CanAccess(int id) => User.IsInRole(RoleNames.Admin) ||
        (User.IsInRole(RoleNames.Company) && int.Parse(User.FindFirstValue("profile_id")!) == id);
}
