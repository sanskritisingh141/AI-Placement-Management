using AIPlacement.Domain.Entities.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPlacement.Infrastructure.Data.Configurations
{
    public class CertificationConfiguration : IEntityTypeConfiguration<Certification>
    {
        public void Configure(EntityTypeBuilder<Certification> builder)
        {
            builder.HasKey(c => c.CertificationId);

            builder.Property(c => c.CertificateName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.IssuingOrganization)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.IssueDate)
                .IsRequired();

            builder.Property(c => c.CredentialUrl)
                .HasMaxLength(500);

            builder.HasOne<StudentProfile>()
                .WithMany()
                .HasForeignKey(c => c.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
