using AiSoftwareFactory.Contracts.Agents;
using AiSoftwareFactory.Domain.Agents;
using AutoMapper;

namespace AiSoftwareFactory.Application.Features.Agents.Mappings;

/// <summary>AutoMapper profile for the Agent feature.</summary>
public sealed class AgentMappingProfile : Profile
{
    /// <summary>Configures domain → contract mappings.</summary>
    public AgentMappingProfile()
    {
        CreateMap<Agent, AgentResponse>()
            .ForCtorParam("id", opt => opt.MapFrom(src => src.Id.Value))
            .ForCtorParam("name", opt => opt.MapFrom(src => src.Name));
    }
}
