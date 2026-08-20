using AIPlacement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIPlacement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<CompanyProfile> CompanyProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CompanyProfile>(entity =>
        {
            entity.ToTable("CompanyProfiles");

            entity.HasKey(x => x.CompanyId);

            entity.Property(x => x.CompanyName)
                .IsRequired();

            entity.Property(x => x.Description);

            entity.Property(x => x.Website);

            entity.Property(x => x.Industry);

            entity.Property(x => x.ContactEmail);

            entity.Property(x => x.ContactPhone);
        });
    }
}