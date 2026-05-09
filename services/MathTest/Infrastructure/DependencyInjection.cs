using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MathTest.Application.Common.Abstractions;
using MathTest.Application.Identity.Interfaces;
using MathTest.Application.Identity.Repositories;
using MathTest.Infrastructure.Options;
using MathTest.Infrastructure.Persistence;
using MathTest.Infrastructure.Persistence.Repositories;
using MathTest.Infrastructure.Security;

namespace MathTest.Infrastructure;

/// <summary>
/// Registers Infrastructure layer services
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is missing or empty.");
        }

        services.AddOptions<DatabaseOptions>()
            .Configure<IConfiguration>((options, config) =>
            {
                options.ConnectionString =
                    config.GetConnectionString("DefaultConnection") ?? string.Empty;
            });

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddEfUnitOfWork<AppDbContext>();

        return services;
    }

    public static IServiceCollection AddEfUnitOfWork<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        services.AddScoped<IUnitOfWork, EfUnitOfWork<TDbContext>>();
        return services;
    }
}
