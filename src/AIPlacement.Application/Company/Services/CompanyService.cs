using AIPlacement.Application.Company.DTOs;
using AIPlacement.Application.Company.Interfaces;
using AIPlacement.Domain.Entities;

namespace AIPlacement.Application.Company.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyService(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<CompanyProfileDto?> GetByIdAsync(int companyId)
    {
        var company =
            await _companyRepository.GetByIdAsync(companyId);

        if (company == null)
            return null;

        return MapToDto(company);
    }

    public async Task<CompanyProfileDto?> GetByUserIdAsync(int userId)
    {
        var companies =
            await _companyRepository.GetAllAsync();

        var company =
            companies.FirstOrDefault(x => x.UserId == userId);

        if (company == null)
            return null;

        return MapToDto(company);
    }

    public async Task<CompanyProfileDto> CreateAsync(
        CompanyProfileDto company)
    {
        var entity = new CompanyProfile
        {
            UserId = company.UserId,
            CompanyName = company.CompanyName,
            Description = company.Description,
            Website = company.Website,
            Industry = company.Industry,
            ContactEmail = company.ContactEmail,
            ContactPhone = company.ContactPhone
        };

        var created =
            await _companyRepository.CreateAsync(entity);

        return MapToDto(created);
    }

    public async Task<CompanyProfileDto?> UpdateAsync(
        int companyId,
        CompanyProfileDto company)
    {
        var entity = new CompanyProfile
        {
            CompanyId = companyId,
            UserId = company.UserId,
            CompanyName = company.CompanyName,
            Description = company.Description,
            Website = company.Website,
            Industry = company.Industry,
            ContactEmail = company.ContactEmail,
            ContactPhone = company.ContactPhone
        };

        var updated =
            await _companyRepository.UpdateAsync(
                companyId,
                entity);

        if (updated == null)
            return null;

        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(int companyId)
    {
        return await _companyRepository.DeleteAsync(companyId);
    }

    private static CompanyProfileDto MapToDto(
        CompanyProfile company)
    {
        return new CompanyProfileDto
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
}