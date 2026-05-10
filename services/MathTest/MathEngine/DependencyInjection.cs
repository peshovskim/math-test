using MathTest.MathEngine.Interfaces;
using MathTest.MathEngine.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MathTest.MathEngine;

public static class DependencyInjection
{
    public static IServiceCollection AddMathEngine(this IServiceCollection services)
    {
        services.AddSingleton<IMathEvaluator, MathEvaluator>();

        return services;
    }
}
