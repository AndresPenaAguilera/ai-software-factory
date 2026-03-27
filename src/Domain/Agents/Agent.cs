using AiSoftwareFactory.Domain.Agents.Events;
using AiSoftwareFactory.Domain.Common;

namespace AiSoftwareFactory.Domain.Agents;

/// <summary>Strongly-typed identifier for an <see cref="Agent"/>.</summary>
public record AgentId(Guid Value)
{
    /// <summary>Creates a new <see cref="AgentId"/> with a random <see cref="Guid"/>.</summary>
    public static AgentId New() => new(Guid.NewGuid());
}

/// <summary>Represents an AI agent with a unique identity and name.</summary>
public sealed class Agent : BaseEntity
{
    /// <summary>Strongly-typed primary key.</summary>
    public AgentId Id { get; private set; } = default!;

    /// <summary>Display name of the agent.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Required by EF Core — do not use in application code.</summary>
    private Agent() { }

    /// <summary>
    /// Factory method that creates a new <see cref="Agent"/> and raises
    /// <see cref="AgentCreatedEvent"/>.
    /// </summary>
    /// <param name="name">The agent's display name.</param>
    public static Agent Create(string name)
    {
        Agent agent = new()
        {
            Id = AgentId.New(),
            Name = name
        };

        agent.AddDomainEvent(new AgentCreatedEvent(agent.Id));

        return agent;
    }
}
