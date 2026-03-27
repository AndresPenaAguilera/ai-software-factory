namespace AiSoftwareFactory.Domain.Common;

/// <summary>Base class for all domain entities. Provides domain event collection.</summary>
public abstract class BaseEntity
{
    private readonly List<object> _domainEvents = [];

    /// <summary>Read-only view of raised domain events.</summary>
    public IReadOnlyList<object> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>Raises a domain event, appending it to the internal collection.</summary>
    protected void AddDomainEvent(object domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>Clears all raised domain events (typically called after dispatch).</summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
