using AIPlacement.Domain.Entities.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPlacement.Infrastructure.Data.Configurations;

public class ApplicationConfiguration
    : IEntityTypeConfiguration<AIPlacement.Domain.Entities.Applications.Application>
{
    public void Configure(
        EntityTypeBuilder<AIPlacement.Domain.Entities.Applications.Application> builder)
    {
        builder.HasKey(x => x.ApplicationId);

        builder.Property(x => x.AppliedAt)
            .IsRequired();

        builder.Property(x => x.CurrentStatus)
            .HasMaxLength(50);

        builder.Property(x => x.RecruiterRemarks);

    

        builder.HasOne<JobDrive>()
            .WithMany()
            .HasForeignKey(x => x.JobDriveId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}