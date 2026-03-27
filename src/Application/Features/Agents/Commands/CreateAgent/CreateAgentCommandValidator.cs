using FluentValidation;

namespace AiSoftwareFactory.Application.Features.Agents.Commands.CreateAgent;

/// <summary>FluentValidation rules for <see cref="CreateAgentCommand"/>.</summary>
public sealed class CreateAgentCommandValidator : AbstractValidator<CreateAgentCommand>
{
    /// <summary>Configures validation rules.</summary>
    public CreateAgentCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");
    }
}
