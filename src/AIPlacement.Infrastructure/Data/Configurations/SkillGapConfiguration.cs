using AIPlacement.Domain.Entities;
using AIPlacement.Domain.Entities.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPlacement.Infrastructure.Data.Configurations;

public class SkillGapConfiguration : IEntityTypeConfiguration<SkillGap>
{
    public void Configure(EntityTypeBuilder<SkillGap> builder)
    {
        builder.HasKey(x => x.SkillGapId);

        builder.Property(x => x.GapLevel)
            .HasMaxLength(30);

        builder.Property(x => x.Recommendation);

        builder.HasOne<JobMatchScore>()
            .WithMany()
            .HasForeignKey(x => x.MatchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Skill>()
            .WithMany()
            .HasForeignKey(x => x.SkillId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}