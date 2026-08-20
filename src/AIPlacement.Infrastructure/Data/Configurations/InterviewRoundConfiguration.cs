using AIPlacement.Domain.Entities.Jobs;
using AIPlacement.Domain.Entities.Recruitment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPlacement.Infrastructure.Data.Configurations;

public class InterviewRoundConfiguration
    : IEntityTypeConfiguration<InterviewRound>
{
    public void Configure(EntityTypeBuilder<InterviewRound> builder)
    {
        builder.HasKey(x => x.RoundId);

        builder.Property(x => x.RoundName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.RoundType)
            .HasMaxLength(50);

        builder.Property(x => x.SequenceNo)
            .IsRequired();

        builder.HasOne<JobDrive>()
            .WithMany()
            .HasForeignKey(x => x.JobDriveId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}