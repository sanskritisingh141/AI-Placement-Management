using AIPlacement.Domain.Entities.AI;
using AIPlacement.Domain.Entities.Resumes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPlacement.Infrastructure.Data.Configurations;

public class ResumeAnalysisConfiguration
    : IEntityTypeConfiguration<ResumeAnalysis>
{
    public void Configure(EntityTypeBuilder<ResumeAnalysis> builder)
    {
        builder.HasKey(x => x.AnalysisId);

        builder.Property(x => x.AnalyzedAt)
            .IsRequired();

        builder.Property(x => x.ExtractedText);

        builder.Property(x => x.Summary);

        builder.Property(x => x.ModelVersion)
            .HasMaxLength(50);

        builder.HasOne<Resume>()
            .WithMany()
            .HasForeignKey(x => x.ResumeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}