using AiSoftwareFactory.Domain.Agents;

namespace AiSoftwareFactory.Application.Common.Interfaces;

/// <summary>Persistence abstraction for the <see cref="Agent"/> aggregate.</summary>
public interface IAgentRepository
{
    /// <summary>Persists a new agent to the store.</summary>
    Task AddAsync(Agent agent, CancellationToken ct = default);

    /// <summary>Returns the agent with the given id, or <c>null</c> if not found.</summary>
    Task<Agent?> GetByIdAsync(AgentId id, CancellationToken ct = default);
}
