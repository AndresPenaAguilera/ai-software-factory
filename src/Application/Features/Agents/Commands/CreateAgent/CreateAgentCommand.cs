using AiSoftwareFactory.Contracts.Agents;
using AiSoftwareFactory.Domain.Common;
using MediatR;

namespace AiSoftwareFactory.Application.Features.Agents.Commands.CreateAgent;

/// <summary>Command to create a new agent with the given name.</summary>
public sealed record CreateAgentCommand(string Name) : IRequest<Result<AgentResponse>>;
