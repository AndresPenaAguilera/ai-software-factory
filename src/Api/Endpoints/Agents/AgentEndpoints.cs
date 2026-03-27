using AiSoftwareFactory.Application.Features.Agents.Commands.CreateAgent;
using AiSoftwareFactory.Contracts.Agents;
using AiSoftwareFactory.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AiSoftwareFactory.Api.Endpoints.Agents;

/// <summary>Minimal-API endpoint definitions for the Agents resource.</summary>
public static class AgentEndpoints
{
    /// <summary>Maps all agent routes onto the provided route group.</summary>
    public static RouteGroupBuilder MapAgentEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", CreateAgent)
             .WithName("CreateAgent");

        return group;
    }

    private static async Task<IResult> CreateAgent(
        [FromBody] CreateAgentRequest request,
        ISender sender,
        CancellationToken ct)
    {
        CreateAgentCommand command = new(request.Name);
        Result<AgentResponse> result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/agents/{result.Value.Id}", result.Value)
            : Results.Problem(
                detail: result.Error.Description,
                statusCode: MapErrorTypeToStatus(result.Error.Type));
    }

    private static int MapErrorTypeToStatus(ErrorType errorType) => errorType switch
    {
        ErrorType.NotFound   => StatusCodes.Status404NotFound,
        ErrorType.Conflict   => StatusCodes.Status409Conflict,
        ErrorType.Validation => StatusCodes.Status422UnprocessableEntity,
        _                    => StatusCodes.Status500InternalServerError
    };
}
