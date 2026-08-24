using AIPlacement.MVC.Models.ApplicationsandRecruitment;
using AIPlacement.MVC.Models.CompanyAndJob;
using AIPlacement.MVC.Models.Placement;
using AllPlacement.MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace AllPlacement.MVC.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<CompanyProfiles> CompanyProfiles { get; set; }

    public DbSet<JobDrive> JobDrives { get; set; }

    public DbSet<JobSkill> JobSkills { get; set; }

    public DbSet<EligibilityCriteria> EligibilityCriteria { get; set; }

    public DbSet<JobEligibleBranch> JobEligibleBranches { get; set; }

    public DbSet<Application> Applications { get; set; }

    public DbSet<ApplicationStatusHistory> ApplicationStatusHistories { get; set; }

    public DbSet<AIPlacement.MVC.Models.ApplicationsandRecruitment.InterviewRound> InterviewRounds { get; set; }

    public DbSet<InterviewSchedule> InterviewSchedules { get; set; }

    public DbSet<InterviewResult> InterviewResults { get; set; }

    public DbSet<PlacementResult> PlacementResults { get; set; }
}
