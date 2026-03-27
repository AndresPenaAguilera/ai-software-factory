using AiSoftwareFactory.Application.Common.Behaviors;
using AiSoftwareFactory.Application.Features.Agents.Commands.CreateAgent;
using AiSoftwareFactory.Application.Features.Agents.Mappings;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AiSoftwareFactory.Application;

/// <summary>Registers all Application-layer services with the DI container.</summary>
public static class DependencyInjection
{
    /// <summary>Adds MediatR, AutoMapper, FluentValidation, and pipeline behaviours.</summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateAgentCommandHandler).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(AgentMappingProfile).Assembly));

        services.AddValidatorsFromAssembly(typeof(CreateAgentCommandValidator).Assembly);

        return services;
    }
}
