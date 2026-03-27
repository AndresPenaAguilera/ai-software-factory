using AiSoftwareFactory.Application.Common.Interfaces;
using AiSoftwareFactory.Domain.Agents;
using AiSoftwareFactory.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AiSoftwareFactory.Infrastructure.Common.Persistence.Repositories;

/// <summary>EF Core implementation of <see cref="IAgentRepository"/>.</summary>
public sealed class AgentRepository(AppDbContext db) : IAgentRepository
{
    /// <inheritdoc/>
    public async Task AddAsync(Agent agent, CancellationToken ct = default)
    {
        await db.Agents.AddAsync(agent, ct);
        await db.SaveChangesAsync(ct);
    }

    /// <inheritdoc/>
    public async Task<Agent?> GetByIdAsync(AgentId id, CancellationToken ct = default)
        => await db.Agents.FirstOrDefaultAsync(a => a.Id == id, ct);
}
