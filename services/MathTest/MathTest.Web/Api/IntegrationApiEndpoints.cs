using Microsoft.Extensions.Options;

namespace MathTest.Web.Api;

internal static class IntegrationApiEndpoints
{
    public static WebApplication MapIntegrationEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("/integration/v1")
            .AddEndpointFilter(ValidateApiKeyAsync);

        group.MapPost("/teacher/exams/batch-xml", ExamXmlBatchUpload.HandleAsync)
            .DisableAntiforgery();

        return app;
    }

    private static async ValueTask<object?> ValidateApiKeyAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        IntegrationApiOptions options =
            context.HttpContext.RequestServices.GetRequiredService<IOptions<IntegrationApiOptions>>().Value;

        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            return TypedResults.Json(
                new { error = "Integration API is not configured (missing Integration:ApiKey)." },
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        if (!context.HttpContext.Request.Headers.TryGetValue("X-Api-Key", out var providedValues))
        {
            return TypedResults.Unauthorized();
        }

        string? provided = providedValues.FirstOrDefault();

        if (string.IsNullOrEmpty(provided)
            || !string.Equals(provided, options.ApiKey, StringComparison.Ordinal))
        {
            return TypedResults.Unauthorized();
        }

        return await next(context);
    }
}
