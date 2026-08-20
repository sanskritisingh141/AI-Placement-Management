using AIPlacement.Domain.Entities;
using AIPlacement.Domain.Entities.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPlacement.Infrastructure.Data.Configurations;

public class JobSkillConfiguration
    : IEntityTypeConfiguration<JobSkill>
{
    public void Configure(EntityTypeBuilder<JobSkill> builder)
    {
        builder.HasKey(x => x.JobSkillId);

        builder.Property(x => x.IsRequired)
            .IsRequired();

        builder.HasOne<JobDrive>()
            .WithMany()
            .HasForeignKey(x => x.JobDriveId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Skill>()
            .WithMany()
            .HasForeignKey(x => x.SkillId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}