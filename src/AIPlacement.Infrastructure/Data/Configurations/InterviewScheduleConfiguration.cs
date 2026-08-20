using AIPlacement.Domain.Entities.Applications;
using AIPlacement.Domain.Entities.Recruitment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPlacement.Infrastructure.Data.Configurations;

public class InterviewScheduleConfiguration
    : IEntityTypeConfiguration<InterviewSchedule>
{
    public void Configure(EntityTypeBuilder<InterviewSchedule> builder)
    {
        builder.HasKey(x => x.InterviewId);

        builder.Property(x => x.ScheduledAt)
            .IsRequired();

        builder.Property(x => x.Location)
            .HasMaxLength(255);

        builder.Property(x => x.MeetingLink)
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .HasMaxLength(30);

        builder.HasOne<AIPlacement.Domain.Entities.Applications.Application>()
            .WithMany()
            .HasForeignKey(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<InterviewRound>()
            .WithMany()
            .HasForeignKey(x => x.RoundId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}