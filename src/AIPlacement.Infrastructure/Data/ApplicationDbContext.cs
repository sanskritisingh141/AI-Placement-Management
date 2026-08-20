using AIPlacement.Domain.Entities.Applications;
using AIPlacement.Domain.Entities.Recruitment;
using Microsoft.EntityFrameworkCore;

namespace AIPlacement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<AIPlacement.Domain.Entities.Applications.Application> Applications { get; set; }

    public DbSet<ApplicationStatusHistory> ApplicationStatusHistories { get; set; }

    public DbSet<InterviewRound> InterviewRounds { get; set; }

    public DbSet<InterviewSchedule> InterviewSchedules { get; set; }

    public DbSet<InterviewResult> InterviewResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfigurationsFromAssembly(
        typeof(ApplicationDbContext).Assembly);
}
}

