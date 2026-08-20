using AIPlacement.Application.Company.DTOs;

namespace AIPlacement.Application.Company.Interfaces;

public interface ICompanyService
{
    Task<CompanyProfileDto?> GetByIdAsync(int companyId);

    Task<CompanyProfileDto?> GetByUserIdAsync(int userId);

    Task<CompanyProfileDto> CreateAsync(CompanyProfileDto company);

    Task<CompanyProfileDto?> UpdateAsync(
        int companyId,
        CompanyProfileDto company);

    Task<bool> DeleteAsync(int companyId);
}