using AIPlacement.Domain.Entities.Applications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPlacement.Infrastructure.Data.Configurations;

public class ApplicationStatusHistoryConfiguration
    : IEntityTypeConfiguration<ApplicationStatusHistory>
{
    public void Configure(EntityTypeBuilder<ApplicationStatusHistory> builder)
    {
        builder.HasKey(x => x.HistoryId);

        builder.Property(x => x.Status)
            .HasMaxLength(50);

        builder.Property(x => x.ChangedAt);

        builder.Property(x => x.Remarks);

        builder.HasOne<AIPlacement.Domain.Entities.Applications.Application>()
            .WithMany()
            .HasForeignKey(x => x.ApplicationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}