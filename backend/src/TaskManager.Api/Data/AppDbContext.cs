using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Entities;

namespace TaskManager.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>(project =>
        {
            project.HasKey(p => p.Id);
            project.Property(p => p.Name).IsRequired().HasMaxLength(120);
            project.Property(p => p.Description).HasMaxLength(2000);
            project.Property(p => p.OwnerId).IsRequired();
            project.HasIndex(p => p.OwnerId);

            project.HasMany(p => p.Tasks)
                .WithOne(t => t.Project)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TaskItem>(task =>
        {
            task.HasKey(t => t.Id);
            task.Property(t => t.Title).IsRequired().HasMaxLength(200);
            task.Property(t => t.Description).HasMaxLength(2000);
            task.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
            task.Property(t => t.Priority).HasConversion<string>().HasMaxLength(20);
            task.HasIndex(t => t.ProjectId);
        });
    }
}
