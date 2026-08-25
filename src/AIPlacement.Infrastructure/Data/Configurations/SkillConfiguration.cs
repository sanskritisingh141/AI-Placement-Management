using AIPlacement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPlacement.Infrastructure.Data.Configurations
{
    public class SkillConfiguration : IEntityTypeConfiguration<Skill>
    {
        public void Configure(EntityTypeBuilder<Skill> builder)
        {
            builder.HasKey(s => s.SkillId);

            builder.Property(s => s.SkillName)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(s => s.SkillName)
                .IsUnique();

            builder.Property(s => s.Category)
                .HasMaxLength(100);
        }
    }
}
