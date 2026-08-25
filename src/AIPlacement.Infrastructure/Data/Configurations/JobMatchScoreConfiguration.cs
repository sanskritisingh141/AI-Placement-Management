using AIPlacement.Domain.Entities;
using AIPlacement.Domain.Entities.AI;
using AIPlacement.Domain.Entities.Jobs;
using AIPlacement.Domain.Entities.Resumes;
using AIPlacement.Domain.Entities.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPlacement.Infrastructure.Data.Configurations;

public class JobMatchScoreConfiguration
    : IEntityTypeConfiguration<JobMatchScore>
{
    public void Configure(EntityTypeBuilder<JobMatchScore> builder)
    {
        builder.HasKey(x => x.MatchId);

        builder.Property(x => x.MatchScore)
            .HasPrecision(5, 2);

        builder.Property(x => x.CalculatedAt)
            .IsRequired();

        builder.HasOne<StudentProfile>()
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<JobDrive>()
            .WithMany()
            .HasForeignKey(x => x.JobDriveId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Resume>()
            .WithMany()
            .HasForeignKey(x => x.ResumeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}