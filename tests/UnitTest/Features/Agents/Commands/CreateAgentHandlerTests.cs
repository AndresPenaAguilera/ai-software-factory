using AiSoftwareFactory.Application.Common.Interfaces;
using AiSoftwareFactory.Application.Features.Agents.Commands.CreateAgent;
using AiSoftwareFactory.Application.Features.Agents.Mappings;
using AiSoftwareFactory.Contracts.Agents;
using AiSoftwareFactory.Domain.Agents;
using AiSoftwareFactory.Domain.Common;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace UnitTest.Features.Agents.Commands;

public sealed class CreateAgentHandlerTests
{
    private readonly IAgentRepository _agentRepository = Substitute.For<IAgentRepository>();
    private readonly IMapper _mapper;
    private readonly CreateAgentCommandHandler _handler;

    public CreateAgentHandlerTests()
    {
        ServiceCollection services = new();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(AgentMappingProfile).Assembly));
        ServiceProvider sp = services.BuildServiceProvider();
        _mapper  = sp.GetRequiredService<IMapper>();
        _handler = new CreateAgentCommandHandler(_agentRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessWithAgentResponse()
    {
        // Arrange
        CreateAgentCommand command = new("Test Agent");

        // Act
        Result<AgentResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Test Agent");
        result.Value.Id.Should().NotBeEmpty();
        await _agentRepository.Received(1)
            .AddAsync(Arg.Any<Agent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidCommand_AgentHasCorrectName()
    {
        // Arrange
        const string agentName = "My AI Agent";
        CreateAgentCommand command = new(agentName);

        // Act
        Result<AgentResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(agentName);
    }

    [Fact]
    public async Task Handle_ValidCommand_RaisesAgentCreatedDomainEvent()
    {
        // Arrange
        CreateAgentCommand command = new("Event Test Agent");
        Agent? capturedAgent = null;

        await _agentRepository
            .AddAsync(Arg.Do<Agent>(a => capturedAgent = a), Arg.Any<CancellationToken>());

        // Act
        Result<AgentResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedAgent.Should().NotBeNull();
        capturedAgent!.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ValidCommand_IdIsNonEmptyGuid()
    {
        // Arrange
        CreateAgentCommand command = new("GUID Test Agent");

        // Act
        Result<AgentResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBe(Guid.Empty);
    }
}
