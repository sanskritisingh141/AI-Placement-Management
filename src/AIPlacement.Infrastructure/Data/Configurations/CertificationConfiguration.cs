using AIPlacement.Domain.Entities.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPlacement.Infrastructure.Data.Configurations;

public class CertificationConfiguration
    : IEntityTypeConfiguration<Certification>
{
    public void Configure(EntityTypeBuilder<Certification> builder)
    {
        builder.HasKey(x => x.CertificationId);

        builder.Property(x => x.CertificateName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.IssuingOrganization)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.IssueDate)
            .IsRequired();

        builder.Property(x => x.CredentialUrl)
            .HasMaxLength(500);

        builder.HasOne<StudentProfile>()
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}