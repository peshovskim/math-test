using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MathTest.Application.Common.Abstractions;
using MathTest.Application.ExamResults;
using MathTest.Application.Identity.Interfaces;
using MathTest.Application.Identity.Repositories;
using MathTest.Application.Interfaces;
using MathTest.Infrastructure.Options;
using MathTest.Infrastructure.Persistence;
using MathTest.Infrastructure.Persistence.Queries;
using MathTest.Infrastructure.Persistence.Repositories;
using MathTest.Infrastructure.Security;
using MathTest.Infrastructure.Xml;

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
        services.AddSingleton<IXmlExamParser, XmlExamParser>();
        services.AddScoped<IExamRepository, ExamRepository>();
        services.AddScoped<IStudentExamResultsQuery, StudentExamResultsQuery>();
        services.AddScoped<ITeacherExamResultsQuery, TeacherExamResultsQuery>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
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
