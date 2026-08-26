using AIPlacement.Application.Certifications.DTOs;
using AIPlacement.Application.Certifications.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AIPlacement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CertificationController : ControllerBase
{
    private readonly ICertificationService _certificationService;

    public CertificationController(
        ICertificationService certificationService)
    {
        _certificationService = certificationService;
    }

    [HttpGet("{certificationId}")]
    public async Task<ActionResult<CertificationDto>> GetById(
        int certificationId)
    {
        var certification =
            await _certificationService.GetByIdAsync(certificationId);

        if (certification == null)
            return NotFound();

        return Ok(certification);
    }

    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<IEnumerable<CertificationDto>>>
        GetByStudentId(int studentId)
    {
        var certifications =
            await _certificationService
                .GetByStudentIdAsync(studentId);

        return Ok(certifications);
    }

    [HttpPost]
    public async Task<ActionResult<CertificationDto>> Create(
        CertificationDto certification)
    {
        var created =
            await _certificationService.CreateAsync(certification);

        return Ok(created);
    }

    [HttpPut("{certificationId}")]
    public async Task<ActionResult<CertificationDto>> Update(
        int certificationId,
        CertificationDto certification)
    {
        var updated =
            await _certificationService.UpdateAsync(
                certificationId,
                certification);

        if (updated == null)
            return NotFound();

        return Ok(updated);
    }

    [HttpDelete("{certificationId}")]
    public async Task<IActionResult> Delete(
        int certificationId)
    {
        var deleted =
            await _certificationService
                .DeleteAsync(certificationId);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
