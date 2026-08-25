using AIPlacement.Domain.Entities;
using AIPlacement.Domain.Entities.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPlacement.Infrastructure.Data.Configurations
{
    public class StudentSkillConfiguration : IEntityTypeConfiguration<StudentSkill>
    {
        public void Configure(EntityTypeBuilder<StudentSkill> builder)
        {
            builder.HasKey(ss => ss.StudentSkillId);

            builder.Property(ss => ss.ProficiencyLevel)
                .HasMaxLength(50);

            builder.HasOne<StudentProfile>()
                .WithMany()
                .HasForeignKey(ss => ss.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Skill>()
                .WithMany()
                .HasForeignKey(ss => ss.SkillId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(ss => new { ss.StudentId, ss.SkillId })
                .IsUnique();
        }
    }
}