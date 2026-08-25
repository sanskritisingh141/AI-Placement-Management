using AIPlacement.Domain.Entities.Resumes;
using AIPlacement.Domain.Entities.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPlacement.Infrastructure.Data.Configurations
{
    public class ResumeConfiguration : IEntityTypeConfiguration<Resume>
    {
        public void Configure(EntityTypeBuilder<Resume> builder)
        {
            builder.HasKey(r => r.ResumeId);

            builder.Property(r => r.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(r => r.FilePath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(r => r.UploadedAt)
                .IsRequired();

            builder.Property(r => r.VersionNo)
                .IsRequired();

            builder.Property(r => r.IsCurrent)
                .IsRequired();

            builder.HasOne<StudentProfile>()
                .WithMany()
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(r => new { r.StudentId, r.VersionNo })
                .IsUnique();
        }
    }
}