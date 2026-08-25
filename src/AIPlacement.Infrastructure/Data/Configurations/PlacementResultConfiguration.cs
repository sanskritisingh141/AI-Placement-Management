using ApplicationEntity = AIPlacement.Domain.Entities.Applications.Application;
using AIPlacement.Domain.Entities.Jobs;
using AIPlacement.Domain.Entities.Placement;
using AIPlacement.Domain.Entities.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPlacement.Infrastructure.Data.Configurations;

public class PlacementResultConfiguration
    : IEntityTypeConfiguration<PlacementResult>
{
    public void Configure(EntityTypeBuilder<PlacementResult> builder)
    {
        builder.HasKey(x => x.PlacementId);

        builder.Property(x => x.PlacementStatus)
            .HasMaxLength(30);

        builder.Property(x => x.Package)
            .HasPrecision(10, 2);

        builder.Property(x => x.PlacementDate)
            .IsRequired();

        builder.Property(x => x.OfferDetails);

        builder.HasOne<StudentProfile>()
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<JobDrive>()
            .WithMany()
            .HasForeignKey(x => x.JobDriveId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationEntity>()
            .WithOne()
            .HasForeignKey<PlacementResult>(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ApplicationId)
            .IsUnique();
    }
}