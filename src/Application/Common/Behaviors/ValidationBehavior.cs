using FluentValidation;
using MediatR;

namespace AiSoftwareFactory.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behaviour that runs all registered FluentValidation validators
/// for a request before the handler is invoked.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <inheritdoc/>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next();

        ValidationContext<TRequest> context = new(request);

        List<FluentValidation.Results.ValidationFailure> failures = (
            await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellationToken))))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await next();

        throw new ValidationException(failures);
    }
}
