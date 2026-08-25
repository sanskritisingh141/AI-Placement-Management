using AIPlacement.Application.Jobs.DTOs;
using AIPlacement.Application.Jobs.Interfaces;
using AIPlacement.Domain.Entities.Jobs;

namespace AIPlacement.Application.Jobs.Services;

public class JobDriveService : IJobDriveService
{
    private readonly IJobDriveRepository _jobDriveRepository;

    public JobDriveService(IJobDriveRepository jobDriveRepository)
    {
        _jobDriveRepository = jobDriveRepository;
    }

    public async Task<IReadOnlyList<JobDriveDto>> GetAvailableAsync()
    {
        var jobDrives = await _jobDriveRepository.GetAvailableAsync();
        return await MapToDtoBatchAsync(jobDrives);
    }

    public async Task<IReadOnlyList<JobDriveDto>> GetByCompanyIdAsync(int companyId)
    {
        var jobDrives = await _jobDriveRepository.GetByCompanyIdAsync(companyId);
        return await MapToDtoBatchAsync(jobDrives);
    }

    public async Task<JobDriveDto?> GetByIdAsync(int jobDriveId)
    {
        var jobDrive = await _jobDriveRepository.GetByIdAsync(jobDriveId);

        if (jobDrive is null)
            return null;

        var criteria = await _jobDriveRepository.GetEligibilityCriteriaAsync(jobDriveId);
        var skills = await _jobDriveRepository.GetJobSkillsAsync(jobDriveId);
        var branches = await _jobDriveRepository.GetEligibleBranchesAsync(jobDriveId);

        return MapToDto(jobDrive, criteria, skills, branches);
    }

    public async Task<JobDriveDto> CreateAsync(CreateJobDriveDto request)
    {
        ValidateRequest(request);

        var jobDrive = new JobDrive
        {
            CompanyId = request.CompanyId,
            JobTitle = request.JobTitle.Trim(),
            JobDescription = request.JobDescription.Trim(),
            Location = request.Location.Trim(),
            MinCGPA = request.MinCGPA,
            GraduationYear = request.GraduationYear,
            SalaryPackage = request.SalaryPackage,
            ApplicationDeadline = request.ApplicationDeadline,
            Status = JobDriveStatus.Draft,
            ApprovalStatus = JobDriveApprovalStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        var eligibilityCriteria = new EligibilityCriteria
        {
            MinCGPA = request.MinCGPA,
            MaxBacklogs = request.MaxBacklogs,
            GraduationYear = request.GraduationYear
        };

        var jobSkills = request.RequiredSkillIds
            .Distinct()
            .Select(skillId => new JobSkill
            {
                SkillId = skillId,
                IsRequired = true
            })
            .ToList();

        var eligibleBranches = request.EligibleBranches
            .Where(branch => !string.IsNullOrWhiteSpace(branch))
            .Select(branch => branch.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(branch => new JobEligibleBranch
            {
                BranchName = branch
            })
            .ToList();

        await _jobDriveRepository.AddAsync(
            jobDrive,
            eligibilityCriteria,
            jobSkills,
            eligibleBranches);

        return MapToDto(jobDrive, eligibilityCriteria, jobSkills, eligibleBranches);
    }

    public async Task<JobDriveDto?> UpdateAsync(int jobDriveId, UpdateJobDriveDto request)
    {
        ValidateRequest(request);

        var jobDrive = await _jobDriveRepository.GetByIdAsync(jobDriveId);

        if (jobDrive is null)
            return null;

        if (jobDrive.Status == JobDriveStatus.Closed)
            throw new InvalidOperationException("A closed job drive cannot be edited.");

        jobDrive.JobTitle = request.JobTitle.Trim();
        jobDrive.JobDescription = request.JobDescription.Trim();
        jobDrive.Location = request.Location.Trim();
        jobDrive.MinCGPA = request.MinCGPA;
        jobDrive.GraduationYear = request.GraduationYear;
        jobDrive.SalaryPackage = request.SalaryPackage;
        jobDrive.ApplicationDeadline = request.ApplicationDeadline;

        var eligibilityCriteria = new EligibilityCriteria
        {
            MinCGPA = request.MinCGPA,
            MaxBacklogs = request.MaxBacklogs,
            GraduationYear = request.GraduationYear
        };

        var jobSkills = request.RequiredSkillIds
            .Distinct()
            .Select(skillId => new JobSkill
            {
                SkillId = skillId,
                IsRequired = true
            })
            .ToList();

        var eligibleBranches = request.EligibleBranches
            .Where(branch => !string.IsNullOrWhiteSpace(branch))
            .Select(branch => branch.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(branch => new JobEligibleBranch
            {
                BranchName = branch
            })
            .ToList();

        await _jobDriveRepository.UpdateAsync(
            jobDrive,
            eligibilityCriteria,
            jobSkills,
            eligibleBranches);

        return MapToDto(jobDrive, eligibilityCriteria, jobSkills, eligibleBranches);
    }

    public async Task<JobDriveDto?> PublishAsync(int jobDriveId)
    {
        var jobDrive = await _jobDriveRepository.GetByIdAsync(jobDriveId);

        if (jobDrive is null)
            return null;

        if (jobDrive.ApprovalStatus != JobDriveApprovalStatus.Approved)
        {
            throw new InvalidOperationException(
                "Only an admin-approved job drive can be published.");
        }

        jobDrive.Status = JobDriveStatus.Open;
        await _jobDriveRepository.UpdateJobDriveAsync(jobDrive);

        var criteria = await _jobDriveRepository.GetEligibilityCriteriaAsync(jobDriveId);
        var skills = await _jobDriveRepository.GetJobSkillsAsync(jobDriveId);
        var branches = await _jobDriveRepository.GetEligibleBranchesAsync(jobDriveId);

        return MapToDto(jobDrive, criteria, skills, branches);
    }

    public async Task<JobDriveDto?> CloseAsync(int jobDriveId)
    {
        var jobDrive = await _jobDriveRepository.GetByIdAsync(jobDriveId);

        if (jobDrive is null)
            return null;

        jobDrive.Status = JobDriveStatus.Closed;
        await _jobDriveRepository.UpdateJobDriveAsync(jobDrive);

        var criteria = await _jobDriveRepository.GetEligibilityCriteriaAsync(jobDriveId);
        var skills = await _jobDriveRepository.GetJobSkillsAsync(jobDriveId);
        var branches = await _jobDriveRepository.GetEligibleBranchesAsync(jobDriveId);

        return MapToDto(jobDrive, criteria, skills, branches);
    }

    private async Task<IReadOnlyList<JobDriveDto>> MapToDtoBatchAsync(
        IReadOnlyList<JobDrive> jobDrives)
    {
        if (jobDrives.Count == 0)
            return [];

        var ids = jobDrives.Select(j => j.JobDriveId).ToList();

        var criteriaByJobDrive = (await _jobDriveRepository
            .GetEligibilityCriteriaBatchAsync(ids))
            .ToDictionary(c => c.JobDriveId);

        var skillsByJobDrive = (await _jobDriveRepository
            .GetJobSkillsBatchAsync(ids))
            .ToLookup(s => s.JobDriveId);

        var branchesByJobDrive = (await _jobDriveRepository
            .GetEligibleBranchesBatchAsync(ids))
            .ToLookup(b => b.JobDriveId);

        return jobDrives.Select(jd => MapToDto(
            jd,
            criteriaByJobDrive.GetValueOrDefault(jd.JobDriveId),
            skillsByJobDrive[jd.JobDriveId].ToList(),
            branchesByJobDrive[jd.JobDriveId].ToList()
        )).ToList();
    }

    private static JobDriveDto MapToDto(
        JobDrive jobDrive,
        EligibilityCriteria? eligibilityCriteria,
        IReadOnlyList<JobSkill> jobSkills,
        IReadOnlyList<JobEligibleBranch> eligibleBranches)
    {
        return new JobDriveDto
        {
            JobDriveId = jobDrive.JobDriveId,
            CompanyId = jobDrive.CompanyId,
            JobTitle = jobDrive.JobTitle,
            JobDescription = jobDrive.JobDescription,
            Location = jobDrive.Location,
            MinCGPA = eligibilityCriteria?.MinCGPA ?? jobDrive.MinCGPA,
            MaxBacklogs = eligibilityCriteria?.MaxBacklogs ?? 0,
            GraduationYear = eligibilityCriteria?.GraduationYear ?? jobDrive.GraduationYear,
            SalaryPackage = jobDrive.SalaryPackage,
            ApplicationDeadline = jobDrive.ApplicationDeadline,
            Status = jobDrive.Status,
            ApprovalStatus = jobDrive.ApprovalStatus,
            CreatedAt = jobDrive.CreatedAt,
            RequiredSkillIds = jobSkills.Select(s => s.SkillId).ToList(),
            EligibleBranches = eligibleBranches.Select(b => b.BranchName).ToList()
        };
    }

    private static void ValidateRequest(CreateJobDriveDto request)
    {
        if (request.CompanyId <= 0)
            throw new ArgumentException("A valid company ID is required.");

        ValidateCommonFields(
            request.JobTitle,
            request.JobDescription,
            request.Location,
            request.MinCGPA,
            request.MaxBacklogs,
            request.GraduationYear,
            request.SalaryPackage,
            request.ApplicationDeadline);
    }

    private static void ValidateRequest(UpdateJobDriveDto request)
    {
        ValidateCommonFields(
            request.JobTitle,
            request.JobDescription,
            request.Location,
            request.MinCGPA,
            request.MaxBacklogs,
            request.GraduationYear,
            request.SalaryPackage,
            request.ApplicationDeadline);
    }

    private static void ValidateCommonFields(
        string jobTitle,
        string jobDescription,
        string location,
        decimal minCgpa,
        int maxBacklogs,
        int graduationYear,
        decimal salaryPackage,
        DateTime applicationDeadline)
    {
        if (string.IsNullOrWhiteSpace(jobTitle))
            throw new ArgumentException("Job title is required.");

        if (string.IsNullOrWhiteSpace(jobDescription))
            throw new ArgumentException("Job description is required.");

        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location is required.");

        if (minCgpa < 0 || minCgpa > 10)
            throw new ArgumentException("Minimum CGPA must be between 0 and 10.");

        if (maxBacklogs < 0)
            throw new ArgumentException("Maximum backlogs cannot be negative.");

        if (graduationYear < DateTime.UtcNow.Year)
            throw new ArgumentException("Graduation year cannot be in the past.");

        if (salaryPackage < 0)
            throw new ArgumentException("Salary package cannot be negative.");

        if (applicationDeadline <= DateTime.UtcNow)
            throw new ArgumentException("Application deadline must be in the future.");
    }
}
