using static MathTest.Web.Api.ApiEndpointResults;

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

        api.MapPost("/register", RegisterAsync).DisableAntiforgery();
        api.MapPost("/login", LoginAsync).DisableAntiforgery();

        return app;
    }

    private static async Task<IResult> RegisterAsync(
        RegisterUserRequest request,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        Result result = await mediator.Send(new RegisterUserCommand(request), cancellationToken);

        return OkOrError(result);
    }

    private static async Task<IResult> LoginAsync(
        LoginUserRequest request,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        Result<LoginResponse> result = await mediator.Send(new LoginUserCommand(request), cancellationToken);

        return OkOrError(result);
    }
}
