using AIPlacement.MVC.Models.ApplicationsandRecruitment;
using AIPlacement.MVC.Models.Placement;
using static System.Net.Mime.MediaTypeNames;

namespace AIPlacement.MVC.Models.CompanyAndJob;

public class JobDrive
{
    public int JobDriveId { get; set; }

    public int CompanyId { get; set; }

    public string JobTitle { get; set; } = null!;

    public string? JobDescription { get; set; }

    public string? Location { get; set; }

    public decimal MinCGPA { get; set; }

    public int GraduationYear { get; set; }

    public decimal SalaryPackage { get; set; }

    public DateTime ApplicationDeadline { get; set; }

    public string? Status { get; set; }

    public string? ApprovalStatus { get; set; }

    public DateTime CreatedAt { get; set; }


    // CompanyProfiles 1 : M JobDrives
    public CompanyProfiles CompanyProfile { get; set; } = null!;


    // JobDrives M : M Skills through JobSkills
    public ICollection<JobSkill> JobSkills { get; set; }
        = new List<JobSkill>();


    // JobDrives 1 : 1 EligibilityCriteria
    public EligibilityCriteria? EligibilityCriteria { get; set; }


    // JobDrives 1 : M JobEligibleBranches
    public ICollection<JobEligibleBranch> JobEligibleBranches { get; set; }
        = new List<JobEligibleBranch>();


    // JobDrives 1 : M Applications
    public ICollection<Application> Applications { get; set; }
        = new List<Application>();


    // JobDrives 1 : M InterviewRounds
    public ICollection<InterviewRound> InterviewRounds { get; set; }
        = new List<InterviewRound>();


    // JobDrives 1 : M PlacementResults
    public ICollection<PlacementResult> PlacementResults { get; set; }
        = new List<PlacementResult>();
}
