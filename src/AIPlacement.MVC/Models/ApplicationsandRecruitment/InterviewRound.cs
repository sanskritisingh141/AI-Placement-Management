using AIPlacement.MVC.Models.CompanyAndJob;

namespace AIPlacement.MVC.Models.ApplicationsandRecruitment;

public class InterviewRound
{
    public int RoundId { get; set; }

    public int JobDriveId { get; set; }

    public string RoundName { get; set; } = null!;

    public string? RoundType { get; set; }

    public int SequenceNo { get; set; }


    // JobDrives 1 : M InterviewRounds
    public JobDrive JobDrive { get; set; } = null!;
}