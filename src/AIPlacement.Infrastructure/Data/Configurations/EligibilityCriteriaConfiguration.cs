using AIPlacement.Domain.Entities.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPlacement.Infrastructure.Data.Configurations;

public class EligibilityCriteriaConfiguration
    : IEntityTypeConfiguration<EligibilityCriteria>
{
    public void Configure(EntityTypeBuilder<EligibilityCriteria> builder)
    {
        builder.HasKey(x => x.EligibilityId);

        builder.Property(x => x.MinCGPA)
            .HasPrecision(4, 2);

        builder.Property(x => x.MaxBacklogs)
            .IsRequired();

        builder.Property(x => x.GraduationYear)
            .IsRequired();

        builder.HasOne<JobDrive>()
            .WithOne()
            .HasForeignKey<EligibilityCriteria>(x => x.JobDriveId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.JobDriveId)
            .IsUnique();
    }
}