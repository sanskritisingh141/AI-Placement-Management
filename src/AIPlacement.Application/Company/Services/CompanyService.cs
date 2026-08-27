using AIPlacement.Application.Company.DTOs;
using AIPlacement.Application.Company.Interfaces;
using AIPlacement.Domain.Entities;

namespace AIPlacement.Application.Company.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _repository;
    public CompanyService(ICompanyRepository repository) => _repository = repository;

    public async Task<CompanyProfileDto?> GetByIdAsync(int companyId)
    {
        if (companyId <= 0) return null;
        var company = await _repository.GetByIdAsync(companyId);
        return company is null ? null : Map(company);
    }

    public async Task<CompanyProfileDto?> GetByUserIdAsync(int userId)
    {
        if (userId <= 0) return null;
        var company = await _repository.GetByUserIdAsync(userId);
        return company is null ? null : Map(company);
    }

    public async Task<CompanyProfileDto> CreateAsync(CompanyProfileDto dto)
    {
        Validate(dto);
        if (!await _repository.UserExistsAsync(dto.UserId))
            throw new ArgumentException("User not found.");
        if (await _repository.GetByUserIdAsync(dto.UserId) is not null)
            throw new InvalidOperationException("A company profile already exists for this user.");
        return Map(await _repository.CreateAsync(ToEntity(dto)));
    }

    public async Task<CompanyProfileDto?> UpdateAsync(int companyId, CompanyProfileDto dto)
    {
        if (companyId <= 0) throw new ArgumentException("A valid company ID is required.");
        Validate(dto);
        var current = await _repository.GetByIdAsync(companyId);
        if (current is null) return null;
        if (current.UserId != dto.UserId)
            throw new InvalidOperationException("A company profile cannot be moved to another user.");
        var updated = await _repository.UpdateAsync(companyId, ToEntity(dto));
        return updated is null ? null : Map(updated);
    }

    public async Task<bool> DeleteAsync(int companyId)
    {
        if (companyId <= 0) throw new ArgumentException("A valid company ID is required.");
        if (await _repository.HasJobDrivesAsync(companyId))
            throw new InvalidOperationException("A company with JobDrives cannot be deleted.");
        return await _repository.DeleteAsync(companyId);
    }

    private static void Validate(CompanyProfileDto dto)
    {
        if (dto.UserId <= 0) throw new ArgumentException("A valid user ID is required.");
        if (string.IsNullOrWhiteSpace(dto.CompanyName)) throw new ArgumentException("Company name is required.");
        if (dto.CompanyName.Trim().Length > 150) throw new ArgumentException("Company name cannot exceed 150 characters.");
        if (dto.Website?.Length > 255) throw new ArgumentException("Website cannot exceed 255 characters.");
        if (dto.Industry?.Length > 100) throw new ArgumentException("Industry cannot exceed 100 characters.");
        if (dto.ContactEmail?.Length > 150) throw new ArgumentException("Contact email cannot exceed 150 characters.");
        if (dto.ContactPhone?.Length > 20) throw new ArgumentException("Contact phone cannot exceed 20 characters.");
    }

    private static CompanyProfile ToEntity(CompanyProfileDto dto) => new()
    {
        CompanyId = dto.CompanyId,
        UserId = dto.UserId,
        CompanyName = dto.CompanyName.Trim(),
        Description = dto.Description?.Trim(),
        Website = dto.Website?.Trim(),
        Industry = dto.Industry?.Trim(),
        ContactEmail = dto.ContactEmail?.Trim(),
        ContactPhone = dto.ContactPhone?.Trim()
    };

    private static CompanyProfileDto Map(CompanyProfile company) => new()
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
