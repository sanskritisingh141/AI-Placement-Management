using AIPlacement.Domain.Entities.Jobs;
using AIPlacement.Domain.Entities.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApplicationEntity = AIPlacement.Domain.Entities.Applications.Application;

namespace AIPlacement.Infrastructure.Data.Configurations;

public class ApplicationConfiguration
    : IEntityTypeConfiguration<ApplicationEntity>
{
    public void Configure(EntityTypeBuilder<ApplicationEntity> builder)
    {
        builder.HasKey(x => x.ApplicationId);

        builder.Property(x => x.AppliedAt)
            .IsRequired();

        builder.Property(x => x.CurrentStatus)
            .HasMaxLength(30);

        builder.Property(x => x.RecruiterRemarks)
            .HasMaxLength(500);

        builder.HasOne<StudentProfile>()
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<JobDrive>()
            .WithMany()
            .HasForeignKey(x => x.JobDriveId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.StudentId, x.JobDriveId })
            .IsUnique();
    }
}