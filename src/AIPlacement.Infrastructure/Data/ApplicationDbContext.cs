using AIPlacement.Domain.Entities;
using AIPlacement.Domain.Entities.Applications;
using AIPlacement.Domain.Entities.Recruitment;
using AIPlacement.Domain.Entities.Resumes;
using AIPlacement.Domain.Entities.Students;
using Microsoft.EntityFrameworkCore;
using ApplicationEntity = AIPlacement.Domain.Entities.Applications.Application;
namespace AIPlacement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<CompanyProfile> CompanyProfiles { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<StudentProfile> StudentProfiles { get; set; }

    public DbSet<StudentSkill> StudentSkills { get; set; }

    public DbSet<Skill> Skills { get; set; }

    public DbSet<Resume> Resumes { get; set; }

    public DbSet<Certification> Certifications { get; set; }

    public DbSet<Project> Projects { get; set; }

    public DbSet<ApplicationEntity> Applications { get; set; }

    public DbSet<ApplicationStatusHistory> ApplicationStatusHistories { get; set; }

    public DbSet<InterviewRound> InterviewRounds { get; set; }

    public DbSet<InterviewSchedule> InterviewSchedules { get; set; }

    public DbSet<InterviewResult> InterviewResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<CompanyProfile>(entity =>
        {
            entity.ToTable("CompanyProfiles");

            entity.HasKey(x => x.CompanyId);

            entity.Property(x => x.CompanyName)
                .IsRequired();

            entity.Property(x => x.Description);

            entity.Property(x => x.Website);

            entity.Property(x => x.Industry);

            entity.Property(x => x.ContactEmail);

            entity.Property(x => x.ContactPhone);
        });
    }
}
