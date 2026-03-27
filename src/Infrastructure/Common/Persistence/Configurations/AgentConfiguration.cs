using AiSoftwareFactory.Domain.Agents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiSoftwareFactory.Infrastructure.Common.Persistence.Configurations;

/// <summary>EF Core fluent configuration for the <see cref="Agent"/> entity.</summary>
public sealed class AgentConfiguration : IEntityTypeConfiguration<Agent>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Agent> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasConversion(
                id => id.Value,
                value => new AgentId(value));

        builder.Property(a => a.Name)
            .HasMaxLength(200)
            .IsRequired();
    }
}
