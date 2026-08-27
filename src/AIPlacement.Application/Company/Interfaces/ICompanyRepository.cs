using AIPlacement.Domain.Entities;

namespace AIPlacement.Application.Company.Interfaces;

public interface ICompanyRepository
{
    Task<CompanyProfile?> GetByIdAsync(int companyId);
    Task<CompanyProfile?> GetByUserIdAsync(int userId);
    Task<bool> UserExistsAsync(int userId);
    Task<bool> HasJobDrivesAsync(int companyId);

    Task<List<CompanyProfile>> GetAllAsync();

    Task<CompanyProfile> CreateAsync(CompanyProfile company);

    Task<CompanyProfile?> UpdateAsync(
        int companyId,
        CompanyProfile company);

    Task<bool> DeleteAsync(int companyId);
}
