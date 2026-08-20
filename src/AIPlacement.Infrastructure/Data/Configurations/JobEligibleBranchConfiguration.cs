using AIPlacement.Domain.Entities.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPlacement.Infrastructure.Data.Configurations;

public class JobEligibleBranchConfiguration
    : IEntityTypeConfiguration<JobEligibleBranch>
{
    public void Configure(EntityTypeBuilder<JobEligibleBranch> builder)
    {
        builder.HasKey(x => x.JobBranchId);

        builder.Property(x => x.BranchName)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne<JobDrive>()
            .WithMany()
            .HasForeignKey(x => x.JobDriveId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}