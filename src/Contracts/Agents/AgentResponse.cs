namespace AiSoftwareFactory.Contracts.Agents;

/// <summary>Represents a created or retrieved agent returned to callers.</summary>
public sealed record AgentResponse(Guid Id, string Name);
