using System.ComponentModel.DataAnnotations;
using AIPlacement.Application.Skills.DTOs;

namespace AIPlacement.MVC.Models.CompanyAndJob;

public class JobDriveFormViewModel
{
    public int? JobDriveId { get; set; }

    [Required]
    public int CompanyId { get; set; }

    [Required(ErrorMessage = "Job title is required.")]
    [StringLength(
        150,
        ErrorMessage = "Job title cannot exceed 150 characters.")]
    [Display(Name = "Job Title")]
    public string JobTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Job description is required.")]
    [Display(Name = "Job Description")]
    public string JobDescription { get; set; } = string.Empty;

    [Required(ErrorMessage = "Location is required.")]
    [StringLength(
        150,
        ErrorMessage = "Location cannot exceed 150 characters.")]
    public string Location { get; set; } = string.Empty;

    [Range(
        typeof(decimal),
        "0",
        "10",
        ErrorMessage = "Minimum CGPA must be between 0 and 10.")]
    [Display(Name = "Minimum CGPA")]
    public decimal MinCGPA { get; set; }

    [Range(
        0,
        100,
        ErrorMessage = "Maximum backlogs cannot be negative.")]
    [Display(Name = "Maximum Backlogs")]
    public int MaxBacklogs { get; set; }

    [Range(
        2000,
        2100,
        ErrorMessage = "Enter a valid graduation year.")]
    [Display(Name = "Graduation Year")]
    public int GraduationYear { get; set; }

    [Range(
        typeof(decimal),
        "0.01",
        "999999999999",
        ErrorMessage = "Salary package must be greater than zero.")]
    [Display(Name = "Salary Package")]
    public decimal SalaryPackage { get; set; }

    [Required(ErrorMessage = "Application deadline is required.")]
    [DataType(DataType.DateTime)]
    [Display(Name = "Application Deadline")]
    public DateTime ApplicationDeadline { get; set; }

    [Display(Name = "Required Skills")]
    public List<int> RequiredSkillIds { get; set; } = [];

    [Display(Name = "Eligible Branches")]
    public List<string> EligibleBranches { get; set; } = [];

    public IReadOnlyList<SkillDto> AvailableSkills { get; set; } = [];

    public IReadOnlyList<string> AvailableBranches { get; set; } =
        ["CSE", "IT", "ECE"];
}