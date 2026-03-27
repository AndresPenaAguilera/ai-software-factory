namespace AiSoftwareFactory.Contracts.Agents;

/// <summary>Payload for the POST /api/agents endpoint.</summary>
public sealed record CreateAgentRequest(string Name);
