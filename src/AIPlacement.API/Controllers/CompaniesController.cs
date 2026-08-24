using AIPlacement.Application.Company.DTOs;
using AIPlacement.Application.Company.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        var company = await _companyService.GetByIdAsync(companyId);

        if (company == null)
            return NotFound();

        return Ok(company);
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var company = await _companyService.GetByUserIdAsync(userId);

        if (company == null)
            return NotFound();

        return Ok(company);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CompanyProfileDto company)
    {
        var createdCompany =
            await _companyService.CreateAsync(company);

        return Ok(createdCompany);
    }

    [HttpPut("{companyId:int}")]
    public async Task<IActionResult> Update(
        int companyId,
        [FromBody] CompanyProfileDto company)
    {
        var updatedCompany =
            await _companyService.UpdateAsync(companyId, company);

        if (updatedCompany == null)
            return NotFound();

        return Ok(updatedCompany);
    }

    [HttpDelete("{companyId:int}")]
    public async Task<IActionResult> Delete(int companyId)
    {
        var deleted = await _companyService.DeleteAsync(companyId);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}