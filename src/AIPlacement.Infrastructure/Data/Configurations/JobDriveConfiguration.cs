using AIPlacement.Domain.Entities;
using AIPlacement.Domain.Entities.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPlacement.Infrastructure.Data.Configurations;

public class JobDriveConfiguration
    : IEntityTypeConfiguration<JobDrive>
{
    public void Configure(EntityTypeBuilder<JobDrive> builder)
    {
        builder.HasKey(x => x.JobDriveId);

        builder.Property(x => x.JobTitle)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.JobDescription);

        builder.Property(x => x.Location)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.MinCGPA)
            .HasPrecision(4, 2);

        builder.Property(x => x.GraduationYear)
            .IsRequired();

        builder.Property(x => x.SalaryPackage)
            .HasPrecision(10, 2);

        builder.Property(x => x.ApplicationDeadline)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.ApprovalStatus)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();
        builder.HasOne<CompanyProfile>()
            .WithMany()
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}