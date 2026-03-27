using AiSoftwareFactory.Api.Endpoints.Agents;
using AiSoftwareFactory.Application;
using AiSoftwareFactory.Infrastructure;
using FluentValidation;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ── Application services ─────────────────────────────────────────────────────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ── Exception handling ───────────────────────────────────────────────────────
builder.Services.AddProblemDetails();

WebApplication app = builder.Build();

// ── Middleware pipeline ───────────────────────────────────────────────────────
app.UseExceptionHandler();
app.UseStatusCodePages();

// Map FluentValidation exceptions to 422 problem responses.
app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (ValidationException ex)
    {
        context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
        await context.Response.WriteAsJsonAsync(new
        {
            type    = "https://tools.ietf.org/html/rfc9110#section-15.5.21",
            title   = "Validation failed.",
            status  = StatusCodes.Status422UnprocessableEntity,
            errors  = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
        });
    }
});

// ── Endpoints ─────────────────────────────────────────────────────────────────
app.MapGroup("/api/agents")
   .MapAgentEndpoints();

app.Run();
