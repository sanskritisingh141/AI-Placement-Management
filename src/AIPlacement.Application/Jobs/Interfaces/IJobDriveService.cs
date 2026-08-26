using AIPlacement.Application.Jobs.DTOs;

namespace AIPlacement.Application.Jobs.Interfaces;

public interface IJobDriveService
{
    Task<IReadOnlyList<JobDriveDto>> GetAvailableAsync();
    Task<IReadOnlyList<JobDriveDto>> GetByCompanyIdAsync(int companyId);
    Task<JobDriveDto?> GetByIdAsync(int jobDriveId);
    Task<JobDriveDto> CreateAsync(CreateJobDriveDto request);
    Task<JobDriveDto?> UpdateAsync(int jobDriveId, UpdateJobDriveDto request);
    Task<JobDriveDto?> PublishAsync(int jobDriveId);
    Task<JobDriveDto?> CloseAsync(int jobDriveId);
}
