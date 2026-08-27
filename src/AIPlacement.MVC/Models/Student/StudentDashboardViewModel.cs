using AIPlacement.Application.Certifications.DTOs;
using AIPlacement.Application.Projects.DTOs;
using AIPlacement.Application.Recruitment.DTOs;
using AIPlacement.Application.Resumes.DTOs;
using AIPlacement.Application.Skills.DTOs;
using AIPlacement.Application.Students.DTOs;

namespace AIPlacement.MVC.Models.Student;

public class StudentDashboardViewModel
{
    public string StudentName { get; set; } = string.Empty;
    public StudentDto Profile { get; set; } = new();
    public IReadOnlyList<SkillDto> Skills { get; set; } = Array.Empty<SkillDto>();
    public IReadOnlyList<ProjectDto> Projects { get; set; } = Array.Empty<ProjectDto>();
    public IReadOnlyList<CertificationDto> Certifications { get; set; } = Array.Empty<CertificationDto>();
    public IReadOnlyList<ResumeDto> Resumes { get; set; } = Array.Empty<ResumeDto>();
    public IReadOnlyList<ApplicantDto> RecentApplications { get; set; } = Array.Empty<ApplicantDto>();
    public int ProfileCompletion { get; set; }
}
