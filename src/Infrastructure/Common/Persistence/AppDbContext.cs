using AiSoftwareFactory.Domain.Agents;
using Microsoft.EntityFrameworkCore;

namespace AiSoftwareFactory.Infrastructure.Common.Persistence;

/// <summary>EF Core database context for the application.</summary>
public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>Agents table.</summary>
    public DbSet<Agent> Agents => Set<Agent>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
