using AiSoftwareFactory.Domain.Common;

namespace AiSoftwareFactory.Domain.Agents;

/// <summary>Centralised error constants for the Agent domain.</summary>
public static class AgentErrors
{
    /// <summary>Returned when an agent cannot be located by its identifier.</summary>
    public static readonly Error NotFound =
        Error.NotFound("Agent.NotFound", "Agent not found.");

    /// <summary>Returned when an agent with the same name already exists.</summary>
    public static readonly Error NameConflict =
        Error.Conflict("Agent.NameConflict", "An agent with this name already exists.");
}
