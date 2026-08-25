using AIPlacement.Domain.Entities.AI;
using AIPlacement.Domain.Entities.Jobs;
using AIPlacement.Domain.Entities.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPlacement.Infrastructure.Data.Configurations
{
    public class AIRecommendationConfiguration
        : IEntityTypeConfiguration<AIRecommendation>
    {
        public void Configure(
            EntityTypeBuilder<AIRecommendation> builder)
        {
            builder.HasKey(r => r.RecommendationId);

            builder.Property(r => r.RecommendationText)
                .IsRequired();

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            builder.HasOne<StudentProfile>()
                .WithMany()
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<JobDrive>()
                .WithMany()
                .HasForeignKey(r => r.JobDriveId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<JobMatchScore>()
                .WithMany()
                .HasForeignKey(r => r.MatchId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}