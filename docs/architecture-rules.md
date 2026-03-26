# Architecture Rules

> This document is the **single source of truth** for architectural decisions.
> Both AI agents and human engineers must follow these rules.
> Last updated: 2026-03-26

---

## 1. Solution structure

```
src/
  Api/                        # Presentation layer
    Endpoints/
      {Feature}/
        {Feature}Endpoints.cs
    Extensions/               # DI, middleware registration
  Application/                # Use-case layer
    Features/
      {Feature}/
        Commands/
          Create{Feature}Command.cs
          Create{Feature}Handler.cs
          Create{Feature}CommandValidator.cs
        Queries/
          Get{Feature}ByIdQuery.cs
          Get{Feature}ByIdHandler.cs
        Mappings/
          {Feature}MappingProfile.cs
    Common/
      Behaviors/              # MediatR pipeline behaviours (logging, validation)
      Interfaces/             # Application-level abstractions (repositories, services)
  Domain/                     # Core business layer — zero external dependencies
    {Feature}/
      {Feature}.cs            # Entity
      {Feature}Errors.cs      # Error constants
      Events/
        {Feature}CreatedEvent.cs
  Infrastructure/             # Data and external services
    Common/
      Persistence/
        AppDbContext.cs
        Repositories/
          {Feature}Repository.cs
  Contracts/                  # Shared data transfer types
    {Feature}/
      {Feature}Request.cs
      {Feature}Response.cs

tests/
  UnitTest/
    Features/{Feature}/
      Commands/
      Queries/
  IntegrationTest/            # Optional — tests against real DB/HTTP
```

---

## 2. Layer dependency rules

```
Api ──────────────────────────────►  Application
                                          │
Infrastructure ──────────────────────►   │
                                          ▼
                                       Domain
```

| Layer | MAY depend on | MUST NOT depend on |
|-------|--------------|-------------------|
| Domain | nothing | Application, Infrastructure, Api |
| Application | Domain, Contracts | Infrastructure, Api |
| Infrastructure | Application, Domain, Contracts | Api |
| Api | Application, Contracts | Domain directly, Infrastructure directly |
| Contracts | nothing | any layer |

**Enforcement:** Add an ArchUnit or NDepend rule in CI to fail on violations.

---

## 3. Naming conventions

| Artefact | Convention | Example |
|----------|-----------|---------|
| Entity | PascalCase noun | `Order`, `Customer` |
| Command | `Verb + Entity + Command` | `CreateOrderCommand` |
| Query | `Get + Entity + By + Field + Query` | `GetOrderByIdQuery` |
| Handler | Command/Query name + `Handler` | `CreateOrderHandler` |
| Validator | Command/Query name + `Validator` | `CreateOrderCommandValidator` |
| Repository interface | `I + Entity + Repository` | `IOrderRepository` |
| Repository impl. | `Entity + Repository` | `OrderRepository` |
| Endpoint class | `Entity + Endpoints` | `OrderEndpoints` |
| DTO request | `Entity + Request` | `CreateOrderRequest` |
| DTO response | `Entity + Response` | `OrderResponse` |
| Domain event | Past-tense description + `Event` | `OrderCreatedEvent` |
| Error constants | `Entity + Errors` static class | `OrderErrors.NotFound` |

---

## 4. Entity conventions

```csharp
// ✅ Correct — rich domain model
public sealed class Order : BaseEntity
{
    private readonly List<OrderLine> _lines = [];

    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();
    public CustomerId CustomerId { get; private set; }
    public OrderStatus Status    { get; private set; } = OrderStatus.Draft;
    public Money Total           { get; private set; } = Money.Zero;

    private Order() { }   // EF Core constructor

    public static Order Create(CustomerId customerId)
    {
        var order = new Order { CustomerId = customerId };
        order.AddDomainEvent(new OrderCreatedEvent(order.Id));
        return order;
    }

    public Result AddLine(ProductId productId, Quantity qty, Price unitPrice)
    {
        if (Status != OrderStatus.Draft)
            return Result.Failure(OrderErrors.CannotModifyConfirmedOrder);

        _lines.Add(OrderLine.Create(Id, productId, qty, unitPrice));
        Total = Money.Sum(_lines.Select(l => l.Subtotal));
        return Result.Success();
    }
}

// ❌ Wrong — anemic model with public setters
public class Order
{
    public Guid Id { get; set; }
    public List<OrderLine> Lines { get; set; } = [];
    public decimal Total { get; set; }
}
```

---

## 5. Repository pattern

```csharp
// Interface in Application/Common/Interfaces/
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(OrderId id, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetByCustomerAsync(CustomerId customerId, CancellationToken ct = default);
    Task AddAsync(Order order, CancellationToken ct = default);
    Task UpdateAsync(Order order, CancellationToken ct = default);
    Task DeleteAsync(Order order, CancellationToken ct = default);
}

// Implementation in Infrastructure/Common/Persistence/Repositories/
public sealed class OrderRepository(AppDbContext db) : IOrderRepository
{
    public async Task<Order?> GetByIdAsync(OrderId id, CancellationToken ct = default)
        => await db.Orders
            .Include(o => o.Lines)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    // ...
}
```

**Rules:**
- Never expose `IQueryable` outside the repository.
- Never inject `AppDbContext` directly into Application handlers.
- Use `Include` only in the repository, never in handlers.

---

## 6. Error handling strategy

```csharp
// Define errors as constants in Domain
public static class OrderErrors
{
    public static readonly Error NotFound =
        Error.NotFound("Order.NotFound", "Order not found.");

    public static readonly Error CannotModifyConfirmedOrder =
        Error.Conflict("Order.AlreadyConfirmed", "Cannot modify a confirmed order.");
}

// Return Result<T> from handlers — never throw for business logic
public async Task<Result<OrderResponse>> Handle(CreateOrderCommand cmd, CancellationToken ct)
{
    var customer = await _customerRepo.GetByIdAsync(cmd.CustomerId, ct);
    if (customer is null)
        return Result.Failure<OrderResponse>(CustomerErrors.NotFound);

    var order = Order.Create(cmd.CustomerId);
    await _orderRepo.AddAsync(order, ct);

    return _mapper.Map<OrderResponse>(order);
}
```

---

## 7. Validation strategy

- FluentValidation validators registered as MediatR pipeline behaviours.
- **No duplication** — if a command is validated by FluentValidation, do NOT
  repeat those checks inside the handler.
- Validate **at the boundary only** (API DTO → Command, or Command itself).

```csharp
// MediatR pipeline behaviour — already registered globally
public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    // ... validates all registered validators before handler is called
}
```

---

## 8. Endpoint conventions (Minimal APIs)

```csharp
public static class OrderEndpoints
{
    public static RouteGroupBuilder MapOrderEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", CreateOrder).RequireAuthorization();
        group.MapGet("/{id:guid}", GetOrderById).RequireAuthorization();
        return group;
    }

    private static async Task<IResult> CreateOrder(
        CreateOrderRequest request,
        ISender sender,
        CancellationToken ct)
    {
        var command = new CreateOrderCommand(request.CustomerId, request.Lines);
        var result = await sender.Send(command, ct);

        return result.IsSuccess
            ? Results.Created($"/api/orders/{result.Value.Id}", result.Value)
            : result.ToProblemResult();   // extension method mapping errors → RFC 7807
    }
}
```

**Rules:**
- Endpoints are **thin** — map request DTO → command/query, send, map result.
- All endpoints require authorisation unless explicitly decorated with
  `[AllowAnonymous]` and documented in this file.
- Route prefix: `/api/{plural-resource}` (e.g., `/api/orders`).

---

## 9. Explicitly public (unauthenticated) endpoints

| Method | Route | Reason |
|--------|-------|--------|
| POST | `/api/auth/login` | Authentication endpoint |
| POST | `/api/auth/register` | Registration endpoint |
| GET | `/api/health` | Health check |

Any new public endpoint must be added to this table before merging.

---

## 10. Technology decisions

| Concern | Library | Notes |
|---------|---------|-------|
| CQRS / Mediator | MediatR 12 | |
| Validation | FluentValidation 11 | Pipeline behaviour |
| ORM | EF Core 8 | Code-first migrations |
| Mapping | AutoMapper 13 | Profile per feature |
| Testing — mocks | NSubstitute 5 | NOT Moq |
| Testing — assertions | FluentAssertions 6 | |
| API documentation | Scalar / NSwag | OpenAPI 3.1 |
| Logging | Serilog | Structured, JSON output |

**Do not introduce new libraries** without updating this table and getting
approval in a PR comment from a maintainer.
