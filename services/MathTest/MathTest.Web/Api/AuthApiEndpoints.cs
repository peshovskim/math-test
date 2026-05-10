using MathTest.Application.Identity.Commands;
using MathTest.Application.Identity.Requests;
using MathTest.Application.Identity.Responses;
using MediatR;
using SharedKernel;

namespace MathTest.Web.Api;

internal static class AuthApiEndpoints
{
    public static WebApplication MapAuthApi(this WebApplication app)
    {
        RouteGroupBuilder api = app.MapGroup("/api/auth");

        api.MapPost("/register", RegisterAsync).DisableAntiforgery().WithName("AuthRegister");

        api.MapPost("/login", LoginAsync).DisableAntiforgery().WithName("AuthLogin");

        return app;
    }

    private static async Task<IResult> RegisterAsync(
        RegisterUserRequest request,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        Result result = await mediator.Send(new RegisterUserCommand(request), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok() : Failure(result);
    }

    private static async Task<IResult> LoginAsync(
        LoginUserRequest request,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        Result<LoginResponse> result = await mediator.Send(new LoginUserCommand(request), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.Value) : Failure(result);
    }

    private static IResult Failure(Result result)
    {
        ResultError error = result.Error!;
        object payload = new { code = error.Code, message = error.Message };

        int statusCode = StatusFor(error.Type);

        return TypedResults.Json(payload, statusCode: statusCode);
    }

    private static int StatusFor(ResultType type) =>
        type switch
        {
            ResultType.NotFound => StatusCodes.Status404NotFound,
            ResultType.Forbidden => StatusCodes.Status403Forbidden,
            ResultType.Conflicted => StatusCodes.Status409Conflict,
            ResultType.Invalid => StatusCodes.Status400BadRequest,
            ResultType.Unauthorized => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError,
        };
}
