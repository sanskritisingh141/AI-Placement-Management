using AIPlacement.Application.Company.DTOs;
using AIPlacement.Application.Company.Interfaces;
using AIPlacement.Application.Company.Services;
using AIPlacement.Domain.Entities;

namespace AIPlacement.Tests.Company;

public class CompanyServiceTests
{
    [Fact]
    public async Task CreateAsync_RejectsUnknownUser()
    {
        var repository = new RepositoryStub { UserExists = false };
        await Assert.ThrowsAsync<ArgumentException>(() =>
            new CompanyService(repository).CreateAsync(ValidDto()));
    }

    [Fact]
    public async Task CreateAsync_RejectsSecondProfileForUser()
    {
        var repository = new RepositoryStub
        {
            UserExists = true,
            ByUser = new CompanyProfile { CompanyId = 1, UserId = 10, CompanyName = "Existing" }
        };
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            new CompanyService(repository).CreateAsync(ValidDto()));
    }

    [Fact]
    public async Task DeleteAsync_RejectsCompanyWithJobDrives()
    {
        var repository = new RepositoryStub { HasJobs = true };
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            new CompanyService(repository).DeleteAsync(1));
    }

    private static CompanyProfileDto ValidDto() => new()
    {
        UserId = 10,
        CompanyName = "Example Company"
    };

    private sealed class RepositoryStub : ICompanyRepository
    {
        public bool UserExists { get; init; }
        public bool HasJobs { get; init; }
        public CompanyProfile? ByUser { get; init; }
        public Task<CompanyProfile?> GetByIdAsync(int id) => Task.FromResult<CompanyProfile?>(null);
        public Task<CompanyProfile?> GetByUserIdAsync(int id) => Task.FromResult(ByUser);
        public Task<bool> UserExistsAsync(int id) => Task.FromResult(UserExists);
        public Task<bool> HasJobDrivesAsync(int id) => Task.FromResult(HasJobs);
        public Task<List<CompanyProfile>> GetAllAsync() => Task.FromResult(new List<CompanyProfile>());
        public Task<CompanyProfile> CreateAsync(CompanyProfile company) => Task.FromResult(company);
        public Task<CompanyProfile?> UpdateAsync(int id, CompanyProfile company) => Task.FromResult<CompanyProfile?>(company);
        public Task<bool> DeleteAsync(int id) => Task.FromResult(true);
    }
}
