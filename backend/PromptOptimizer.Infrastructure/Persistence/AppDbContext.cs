using Microsoft.EntityFrameworkCore;
using PromptOptimizer.Domain.Entities;

namespace PromptOptimizer.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<PromptOptimization> PromptOptimizations => Set<PromptOptimization>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PromptOptimization>(entity =>
        {
            entity.ToTable("PromptOptimizations");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.OriginalPrompt).IsRequired();
            entity.Property(x => x.OptimizedPrompt).IsRequired();
            entity.Property(x => x.Improvements).IsRequired();
            entity.Property(x => x.OptimizationType).HasConversion<string>().HasMaxLength(50);
            entity.Property(x => x.UserId).HasMaxLength(200);
            entity.HasIndex(x => x.CreatedAtUtc);
        });
    }
}
