using AIPlacement.MVC.Models.CompanyAndJob;

namespace AIPlacement.MVC.Models.ApplicationsandRecruitment;

public class Application
{
    public int ApplicationId { get; set; }

    public int StudentId { get; set; }

    public int JobDriveId { get; set; }

    public DateTime AppliedAt { get; set; }

    public string? CurrentStatus { get; set; }

    public string? RecruiterRemarks { get; set; }


    // StudentProfiles 1 : M Applications
    //public StudentProfile StudentProfile { get; set; } = null!;


    // JobDrives 1 : M Applications
    public JobDrive JobDrive { get; set; } = null!;


    // Applications 1 : M ApplicationStatusHistory
    public ICollection<ApplicationStatusHistory> ApplicationStatusHistories
    {
        get;
        set;
    } = new List<ApplicationStatusHistory>();


    // Applications 1 : M InterviewSchedules
    public ICollection<InterviewSchedule> InterviewSchedules
    {
        get;
        set;
    } = new List<InterviewSchedule>();
}
