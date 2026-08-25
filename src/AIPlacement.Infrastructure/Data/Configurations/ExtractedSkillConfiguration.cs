using AIPlacement.Domain.Entities;
using AIPlacement.Domain.Entities.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPlacement.Infrastructure.Data.Configurations;

public class ExtractedSkillConfiguration
    : IEntityTypeConfiguration<ExtractedSkill>
{
    public void Configure(EntityTypeBuilder<ExtractedSkill> builder)
    {
        builder.HasKey(x => x.ExtractedSkillId);

        builder.Property(x => x.ConfidenceScore)
            .HasPrecision(5, 2);

        builder.HasOne<ResumeAnalysis>()
            .WithMany()
            .HasForeignKey(x => x.AnalysisId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Skill>()
            .WithMany()
            .HasForeignKey(x => x.SkillId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}