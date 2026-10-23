using Microsoft.EntityFrameworkCore;
using QuestBase.Api.Domain.Projects;
using QuestBase.Api.Domain.Users;

namespace QuestBase.Api.Data;

public class QuestBaseDbContext : DbContext
{
    public QuestBaseDbContext(DbContextOptions<QuestBaseDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ApiKey> ApiKeys => Set<ApiKey>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasIndex(x => new { x.OwnerUserId, x.Slug }).IsUnique();

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.OwnerUserId);
        });

        modelBuilder.Entity<ApiKey>(entity =>
        {
            entity.HasIndex(x => x.KeyHash).IsUnique();

            entity.HasOne<Project>()
                .WithMany()
                .HasForeignKey(x => x.ProjectId);
        });
    }
}