using AIPlacement.Application.Admin.DTOs;
using AIPlacement.Application.Admin.Interfaces;
using AIPlacement.Application.Authentication;
using AIPlacement.Application.Jobs;
using AIPlacement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AIPlacement.Infrastructure.Admin;

public class AdminRepository : IAdminRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AdminRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<UserRecordDto>> GetStudentsAsync()
    {
        return await (
            from user in _dbContext.Users.AsNoTracking()
            join role in _dbContext.Roles.AsNoTracking() on user.RoleId equals role.RoleId
            join profile in _dbContext.StudentProfiles.AsNoTracking() on user.UserId equals profile.UserId
            where role.RoleName == RoleNames.Student
            orderby user.Name
            select new UserRecordDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = role.RoleName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                RollNo = profile.RollNo,
                Branch = profile.Branch,
                CGPA = profile.CGPA
            }).ToListAsync();
    }

    public async Task<IReadOnlyList<UserRecordDto>> GetCompaniesAsync()
    {
        return await (
            from user in _dbContext.Users.AsNoTracking()
            join role in _dbContext.Roles.AsNoTracking() on user.RoleId equals role.RoleId
            join profile in _dbContext.CompanyProfiles.AsNoTracking() on user.UserId equals profile.UserId
            where role.RoleName == RoleNames.Company
            orderby profile.CompanyName
            select new UserRecordDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = role.RoleName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                CompanyName = profile.CompanyName,
                Industry = profile.Industry
            }).ToListAsync();
    }

    public async Task<UserRecordDto?> GetUserAsync(int userId)
    {
        var students = await GetStudentsAsync();
        var student = students.FirstOrDefault(user => user.UserId == userId);
        if (student is not null)
            return student;

        var companies = await GetCompaniesAsync();
        return companies.FirstOrDefault(user => user.UserId == userId);
    }

    public async Task<bool> SetUserActiveAsync(int userId, bool isActive)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user is null)
            return false;

        user.IsActive = isActive;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyList<JobDriveApprovalDto>> GetJobDrivesAsync(bool pendingOnly)
    {
        var query =
            from job in _dbContext.JobDrives.AsNoTracking()
            join company in _dbContext.CompanyProfiles.AsNoTracking()
                on job.CompanyId equals company.CompanyId
            select new JobDriveApprovalDto
            {
                JobDriveId = job.JobDriveId,
                CompanyId = job.CompanyId,
                CompanyName = company.CompanyName,
                JobTitle = job.JobTitle,
                Location = job.Location,
                MinCGPA = job.MinCGPA,
                SalaryPackage = job.SalaryPackage,
                ApplicationDeadline = job.ApplicationDeadline,
                Status = job.Status,
                ApprovalStatus = job.ApprovalStatus,
                CreatedAt = job.CreatedAt
            };

        if (pendingOnly)
            query = query.Where(job => job.ApprovalStatus == JobDriveApprovalStatus.Pending);

        return await query.OrderByDescending(job => job.CreatedAt).ToListAsync();
    }

    public async Task<JobDriveApprovalDto?> SetJobDriveApprovalAsync(
        int jobDriveId,
        string approvalStatus)
    {
        var job = await _dbContext.JobDrives.FindAsync(jobDriveId);
        if (job is null)
            return null;

        job.ApprovalStatus = approvalStatus;
        if (approvalStatus == JobDriveApprovalStatus.Rejected)
            job.Status = JobDriveStatus.Draft;

        await _dbContext.SaveChangesAsync();

        return (await GetJobDrivesAsync(false))
            .First(item => item.JobDriveId == jobDriveId);
    }

    public async Task<IReadOnlyList<ApplicationMonitorDto>> GetApplicationsAsync(string? status = null)
    {
        var query =
            from application in _dbContext.Applications.AsNoTracking()
            join student in _dbContext.StudentProfiles.AsNoTracking()
                on application.StudentId equals student.StudentId
            join user in _dbContext.Users.AsNoTracking()
                on student.UserId equals user.UserId
            join job in _dbContext.JobDrives.AsNoTracking()
                on application.JobDriveId equals job.JobDriveId
            join company in _dbContext.CompanyProfiles.AsNoTracking()
                on job.CompanyId equals company.CompanyId
            select new ApplicationMonitorDto
            {
                ApplicationId = application.ApplicationId,
                StudentId = application.StudentId,
                StudentName = user.Name,
                RollNo = student.RollNo,
                Branch = student.Branch,
                JobDriveId = job.JobDriveId,
                JobTitle = job.JobTitle,
                CompanyName = company.CompanyName,
                AppliedAt = application.AppliedAt,
                CurrentStatus = application.CurrentStatus ?? "Applied",
                RecruiterRemarks = application.RecruiterRemarks
            };

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(application => application.CurrentStatus == status);

        return await query.OrderByDescending(application => application.AppliedAt).ToListAsync();
    }

    public async Task<IReadOnlyList<PlacementRecordDto>> GetPlacementsAsync()
    {
        return await (
            from placement in _dbContext.PlacementResults.AsNoTracking()
            join student in _dbContext.StudentProfiles.AsNoTracking()
                on placement.StudentId equals student.StudentId
            join user in _dbContext.Users.AsNoTracking()
                on student.UserId equals user.UserId
            join job in _dbContext.JobDrives.AsNoTracking()
                on placement.JobDriveId equals job.JobDriveId
            join company in _dbContext.CompanyProfiles.AsNoTracking()
                on job.CompanyId equals company.CompanyId
            orderby placement.PlacementDate descending
            select new PlacementRecordDto
            {
                PlacementId = placement.PlacementId,
                StudentId = placement.StudentId,
                ApplicationId = placement.ApplicationId,
                StudentName = user.Name,
                RollNo = student.RollNo,
                Branch = student.Branch,
                JobDriveId = job.JobDriveId,
                JobTitle = job.JobTitle,
                CompanyName = company.CompanyName,
                PlacementStatus = placement.PlacementStatus ?? "Offered",
                Package = placement.Package,
                PlacementDate = DateOnly.FromDateTime(placement.PlacementDate)
            }).ToListAsync();
    }
}
