using AiSoftwareFactory.Application.Common.Interfaces;
using AiSoftwareFactory.Contracts.Agents;
using AiSoftwareFactory.Domain.Agents;
using AiSoftwareFactory.Domain.Common;
using AutoMapper;
using MediatR;

namespace AiSoftwareFactory.Application.Features.Agents.Commands.CreateAgent;

/// <summary>
/// Handles <see cref="CreateAgentCommand"/>: creates the agent aggregate,
/// persists it, and returns a mapped <see cref="AgentResponse"/>.
/// </summary>
public sealed class CreateAgentCommandHandler(
    IAgentRepository agentRepository,
    IMapper mapper)
    : IRequestHandler<CreateAgentCommand, Result<AgentResponse>>
{
    /// <inheritdoc/>
    public async Task<Result<AgentResponse>> Handle(
        CreateAgentCommand command,
        CancellationToken ct)
    {
        Agent agent = Agent.Create(command.Name);

        await agentRepository.AddAsync(agent, ct);

        AgentResponse response = mapper.Map<AgentResponse>(agent);

        return response;
    }
}
