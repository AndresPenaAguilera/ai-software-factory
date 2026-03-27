namespace AiSoftwareFactory.Domain.Agents.Events;

/// <summary>Raised when a new Agent is successfully created.</summary>
public sealed record AgentCreatedEvent(AgentId AgentId);
