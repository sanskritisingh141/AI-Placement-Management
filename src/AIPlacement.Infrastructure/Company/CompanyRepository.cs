using AIPlacement.Application.Company.Interfaces;
using AIPlacement.Domain.Entities;
using AIPlacement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AIPlacement.Infrastructure.Company;

public class CompanyRepository : ICompanyRepository
{
    private readonly ApplicationDbContext _context;

    public CompanyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CompanyProfile?> GetByIdAsync(int companyId)
    {
        return await _context.CompanyProfiles
            .FirstOrDefaultAsync(x => x.CompanyId == companyId);
    }

    public async Task<CompanyProfile?> GetByUserIdAsync(int userId)
    {
        return await _context.CompanyProfiles
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public Task<bool> UserExistsAsync(int userId) =>
        _context.Users.AnyAsync(user => user.UserId == userId);

    public Task<bool> HasJobDrivesAsync(int companyId) =>
        _context.JobDrives.AnyAsync(job => job.CompanyId == companyId);

    public async Task<List<CompanyProfile>> GetAllAsync()
    {
        return await _context.CompanyProfiles
            .ToListAsync();
    }

    public async Task<CompanyProfile> CreateAsync(
        CompanyProfile company)
    {
        _context.CompanyProfiles.Add(company);

        await _context.SaveChangesAsync();

        return company;
    }

    public async Task<CompanyProfile?> UpdateAsync(
        int companyId,
        CompanyProfile company)
    {
        var existing = await _context.CompanyProfiles
            .FirstOrDefaultAsync(x => x.CompanyId == companyId);

        if (existing == null)
            return null;

        existing.UserId = company.UserId;
        existing.CompanyName = company.CompanyName;
        existing.Description = company.Description;
        existing.Website = company.Website;
        existing.Industry = company.Industry;
        existing.ContactEmail = company.ContactEmail;
        existing.ContactPhone = company.ContactPhone;

        await _context.SaveChangesAsync();

        return existing;
    }

    public async Task<bool> DeleteAsync(int companyId)
    {
        var company = await _context.CompanyProfiles
            .FirstOrDefaultAsync(x => x.CompanyId == companyId);

        if (company == null)
            return false;

        _context.CompanyProfiles.Remove(company);

        await _context.SaveChangesAsync();

        return true;
    }
}
