using AIPlacement.Domain.Entities;
using AIPlacement.Domain.Entities.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPlacement.Infrastructure.Data.Configurations
{
    public class StudentProfileConfiguration : IEntityTypeConfiguration<StudentProfile>
    {
        public void Configure(EntityTypeBuilder<StudentProfile> builder)
        {
            builder.HasKey(s => s.StudentId);

            builder.Property(s => s.RollNo)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(s => s.RollNo)
                .IsUnique();

            builder.Property(s => s.Branch)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.CGPA)
                .HasPrecision(4, 2);

            builder.Property(s => s.GraduationYear)
                .IsRequired();

            builder.Property(s => s.Phone)
                .HasMaxLength(20);

            builder.Property(s => s.Phone)
                .HasMaxLength(20);

            builder.Property(s => s.DateOfBirth)
                .IsRequired(false);

            builder.Property(s => s.CreatedAt)
                .IsRequired();

            builder.Property(s => s.CreatedAt)
                .IsRequired();

            builder.HasOne<User>()
                .WithOne()
                .HasForeignKey<StudentProfile>(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
