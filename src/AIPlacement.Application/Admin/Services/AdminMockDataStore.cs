using AIPlacement.Application.Admin.DTOs;

namespace AIPlacement.Application.Admin.Services;

/// <summary>
/// Temporary in-memory data used by every Admin + Analytics service so the module is
/// demoable end-to-end before the shared SQL Server database / EF Core DbContext
/// (Shared Foundation TSK-01..TSK-04) is wired up.
///
/// Once the real DbContext exists, each service in this folder should be rewritten to
/// query it instead of this class, without changing the public interfaces or the API /
/// MVC controllers that depend on them.
/// </summary>
public static class AdminMockDataStore
{
    private static readonly object SyncRoot = new();

    public static List<UserRecordDto> Students { get; } = new()
    {
        new UserRecordDto { UserId = 101, Name = "Anshika Srivastava", Email = "anshika@college.edu", Role = "Student", IsActive = true, CreatedAt = new DateTime(2026, 8, 13), RollNo = "IN26014085", Branch = "CSE", CGPA = 8.7m },
        new UserRecordDto { UserId = 102, Name = "Sanskriti Singh", Email = "sanskriti@college.edu", Role = "Student", IsActive = true, CreatedAt = new DateTime(2026, 8, 13), RollNo = "IN26014091", Branch = "CSE", CGPA = 9.1m },
        new UserRecordDto { UserId = 103, Name = "Rohan Mehta", Email = "rohan@college.edu", Role = "Student", IsActive = true, CreatedAt = new DateTime(2026, 8, 14), RollNo = "IN26013210", Branch = "IT", CGPA = 8.2m },
        new UserRecordDto { UserId = 104, Name = "Priya Nair", Email = "priya@college.edu", Role = "Student", IsActive = true, CreatedAt = new DateTime(2026, 8, 14), RollNo = "IN26013452", Branch = "ECE", CGPA = 7.6m },
        new UserRecordDto { UserId = 105, Name = "Aditya Rao", Email = "aditya@college.edu", Role = "Student", IsActive = false, CreatedAt = new DateTime(2026, 8, 15), RollNo = "IN26013980", Branch = "IT", CGPA = 6.9m },
        new UserRecordDto { UserId = 106, Name = "Meera Joshi", Email = "meera@college.edu", Role = "Student", IsActive = true, CreatedAt = new DateTime(2026, 8, 15), RollNo = "IN26014410", Branch = "CSE", CGPA = 8.9m },
    };

    public static List<UserRecordDto> Recruiters { get; } = new()
    {
        new UserRecordDto { UserId = 201, Name = "Kavya Menon", Email = "hr@brightsoft.com", Role = "Company", IsActive = true, CreatedAt = new DateTime(2026, 8, 13), CompanyName = "BrightSoft Technologies", Industry = "IT Services" },
        new UserRecordDto { UserId = 202, Name = "Arjun Verma", Email = "talent@nexawave.io", Role = "Company", IsActive = true, CreatedAt = new DateTime(2026, 8, 14), CompanyName = "NexaWave Analytics", Industry = "Data & AI" },
        new UserRecordDto { UserId = 203, Name = "Sneha Kapoor", Email = "careers@finedgeindia.com", Role = "Company", IsActive = true, CreatedAt = new DateTime(2026, 8, 16), CompanyName = "FinEdge India", Industry = "FinTech" },
    };

    public static List<JobDriveApprovalDto> JobDrives { get; } = new()
    {
        new JobDriveApprovalDto { JobDriveId = 301, CompanyId = 201, CompanyName = "BrightSoft Technologies", JobTitle = "Associate Software Engineer", Location = "Bengaluru", MinCGPA = 7.5m, SalaryPackage = 650000m, ApplicationDeadline = new DateTime(2026, 8, 22), Status = "Published", ApprovalStatus = "Approved", CreatedAt = new DateTime(2026, 8, 16) },
        new JobDriveApprovalDto { JobDriveId = 302, CompanyId = 202, CompanyName = "NexaWave Analytics", JobTitle = "Data Analyst Trainee", Location = "Remote", MinCGPA = 8.0m, SalaryPackage = 720000m, ApplicationDeadline = new DateTime(2026, 8, 23), Status = "Published", ApprovalStatus = "Pending", CreatedAt = new DateTime(2026, 8, 18) },
        new JobDriveApprovalDto { JobDriveId = 303, CompanyId = 203, CompanyName = "FinEdge India", JobTitle = "Backend Developer Intern", Location = "Pune", MinCGPA = 7.0m, SalaryPackage = 500000m, ApplicationDeadline = new DateTime(2026, 8, 24), Status = "Draft", ApprovalStatus = "Pending", CreatedAt = new DateTime(2026, 8, 19) },
    };

    public static List<ApplicationMonitorDto> Applications { get; } = new()
    {
        new ApplicationMonitorDto { ApplicationId = 401, StudentId = 101, StudentName = "Anshika Srivastava", RollNo = "IN26014085", Branch = "CSE", JobDriveId = 301, JobTitle = "Associate Software Engineer", CompanyName = "BrightSoft Technologies", AppliedAt = new DateTime(2026, 8, 19), CurrentStatus = "Interview" },
        new ApplicationMonitorDto { ApplicationId = 402, StudentId = 102, StudentName = "Sanskriti Singh", RollNo = "IN26014091", Branch = "CSE", JobDriveId = 301, JobTitle = "Associate Software Engineer", CompanyName = "BrightSoft Technologies", AppliedAt = new DateTime(2026, 8, 19), CurrentStatus = "Selected" },
        new ApplicationMonitorDto { ApplicationId = 403, StudentId = 106, StudentName = "Meera Joshi", RollNo = "IN26014410", Branch = "CSE", JobDriveId = 301, JobTitle = "Associate Software Engineer", CompanyName = "BrightSoft Technologies", AppliedAt = new DateTime(2026, 8, 20), CurrentStatus = "Shortlisted" },
        new ApplicationMonitorDto { ApplicationId = 404, StudentId = 103, StudentName = "Rohan Mehta", RollNo = "IN26013210", Branch = "IT", JobDriveId = 302, JobTitle = "Data Analyst Trainee", CompanyName = "NexaWave Analytics", AppliedAt = new DateTime(2026, 8, 20), CurrentStatus = "Applied" },
        new ApplicationMonitorDto { ApplicationId = 405, StudentId = 104, StudentName = "Priya Nair", RollNo = "IN26013452", Branch = "ECE", JobDriveId = 302, JobTitle = "Data Analyst Trainee", CompanyName = "NexaWave Analytics", AppliedAt = new DateTime(2026, 8, 21), CurrentStatus = "Rejected" },
    };

    public static List<PlacementRecordDto> Placements { get; } = new()
    {
        new PlacementRecordDto { PlacementId = 501, StudentId = 102, StudentName = "Sanskriti Singh", RollNo = "IN26014091", Branch = "CSE", JobDriveId = 301, JobTitle = "Associate Software Engineer", CompanyName = "BrightSoft Technologies", PlacementStatus = "Confirmed", Package = 650000m, PlacementDate = new DateOnly(2026, 8, 21) },
    };

    public static void SetActiveStatus(int userId, bool isActive)
    {
        lock (SyncRoot)
        {
            var user = Students.FirstOrDefault(s => s.UserId == userId)
                       ?? Recruiters.FirstOrDefault(r => r.UserId == userId);

            if (user != null)
                user.IsActive = isActive;
        }
    }

    public static JobDriveApprovalDto? SetApproval(int jobDriveId, string approvalStatus, string? reason)
    {
        lock (SyncRoot)
        {
            var drive = JobDrives.FirstOrDefault(d => d.JobDriveId == jobDriveId);

            if (drive == null)
                return null;

            drive.ApprovalStatus = approvalStatus;
            drive.RejectionReason = reason;

            if (approvalStatus == "Approved")
                drive.Status = "Published";

            return drive;
        }
    }
}
