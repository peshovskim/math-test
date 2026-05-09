using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MathTest.Application;

/// <summary>
/// Registers Application layer services
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}
