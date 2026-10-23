using Microsoft.EntityFrameworkCore;
using QuestBase.Api.Entities;

namespace QuestBase.Api.Data;

public class QuestBaseDbContext : DbContext
{
    public QuestBaseDbContext(DbContextOptions<QuestBaseDbContext> options)
        : base(options) { }

    public DbSet<Project> Projects => Set<Project>();
}