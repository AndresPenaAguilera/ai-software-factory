using AiSoftwareFactory.Application.Common.Interfaces;
using AiSoftwareFactory.Infrastructure.Common.Persistence;
using AiSoftwareFactory.Infrastructure.Common.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiSoftwareFactory.Infrastructure;

/// <summary>Registers all Infrastructure-layer services with the DI container.</summary>
public static class DependencyInjection
{
    /// <summary>Adds EF Core DbContext and repository implementations.</summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IAgentRepository, AgentRepository>();

        return services;
    }
}
