using AIPlacement.Domain.Entities.Recruitment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPlacement.Infrastructure.Data.Configurations;

public class InterviewResultConfiguration
    : IEntityTypeConfiguration<InterviewResult>
{
    public void Configure(EntityTypeBuilder<InterviewResult> builder)
    {
        builder.HasKey(x => x.ResultId);

        builder.Property(x => x.Result)
            .HasMaxLength(50);

        builder.Property(x => x.Score)
            .HasPrecision(5, 2);

        builder.Property(x => x.Remarks);

        builder.Property(x => x.EvaluatedAt);

        builder.HasOne<InterviewSchedule>()
            .WithOne()
            .HasForeignKey<InterviewResult>(x => x.InterviewId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.InterviewId)
            .IsUnique();
    }
}