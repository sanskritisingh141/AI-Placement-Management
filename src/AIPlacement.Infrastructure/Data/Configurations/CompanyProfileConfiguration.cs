using AIPlacement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIPlacement.Infrastructure.Data.Configurations;

public class CompanyProfileConfiguration : IEntityTypeConfiguration<CompanyProfile>
{
    public void Configure(EntityTypeBuilder<CompanyProfile> builder)
    {
        builder.HasKey(x => x.CompanyId);

        builder.Property(x => x.CompanyName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Description);

        builder.Property(x => x.Website)
            .HasMaxLength(255);

        builder.Property(x => x.Industry)
            .HasMaxLength(100);

        builder.Property(x => x.ContactEmail)
            .HasMaxLength(150);

        builder.Property(x => x.ContactPhone)
            .HasMaxLength(20);

        builder.HasIndex(x => x.UserId)
            .IsUnique();

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<CompanyProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}