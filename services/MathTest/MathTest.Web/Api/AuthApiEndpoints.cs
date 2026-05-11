using System.Security.Claims;
using static MathTest.Web.Api.ApiEndpointResults;

using MathTest.Application.Identity.Commands;
using MathTest.Application.Identity.Requests;
using MathTest.Application.Identity.Responses;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SharedKernel;

namespace MathTest.Web.Api;

internal static class AuthApiEndpoints
{
    public static WebApplication MapAuthApi(this WebApplication app)
    {
        RouteGroupBuilder api = app.MapGroup("/api/auth");

        api.MapPost("/register", RegisterAsync).DisableAntiforgery();
        api.MapPost("/login", LoginAsync).DisableAntiforgery();
        api.MapPost("/logout", LogoutAsync).DisableAntiforgery();

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
        HttpContext httpContext,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        (LoginUserRequest? request, string? returnUrl) =
            await ReadLoginRequestAsync(httpContext, cancellationToken);

        if (request is null)
        {
            return TypedResults.BadRequest();
        }

        Result<LoginResponse> result =
            await mediator.Send(
                new LoginUserCommand(request),
                cancellationToken);

        if (!result.IsSuccess)
        {
            if (httpContext.Request.HasFormContentType)
            {
                return TypedResults.Redirect(
                    $"/login?failed=1&ReturnUrl={Uri.EscapeDataString(SafeReturnUrl(returnUrl))}");
            }

            return OkOrError(result);
        }

        await SignInWithCookieAsync(httpContext, result.Value!);

        if (httpContext.Request.HasFormContentType)
        {
            return TypedResults.Redirect(SafeReturnUrl(returnUrl));
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task LogoutAsync(HttpContext ctx)
    {
        await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        ctx.Response.Redirect("/");
    }

    private static async Task<(LoginUserRequest? Request, string? ReturnUrl)>
        ReadLoginRequestAsync(
            HttpContext httpContext,
            CancellationToken cancellationToken)
    {
        if (httpContext.Request.HasFormContentType)
        {
            IFormCollection form =
                await httpContext.Request.ReadFormAsync(cancellationToken);

            return (
                new LoginUserRequest
                {
                    Email = form["Email"].ToString(),
                    Password = form["Password"].ToString(),
                },
                form["ReturnUrl"].ToString());
        }

        LoginUserRequest? request =
            await httpContext.Request.ReadFromJsonAsync<LoginUserRequest>(
                cancellationToken: cancellationToken);

        return (request, null);
    }

    private static async Task SignInWithCookieAsync(
        HttpContext httpContext,
        LoginResponse login)
    {
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, login.UserId.ToString()),
            new(ClaimTypes.Email, login.Email),
            new(ClaimTypes.Name, $"{login.FirstName} {login.LastName}"),
        ];

        claims.AddRange(
            login.RoleNames.Select(role =>
                new Claim(ClaimTypes.Role, role)));

        ClaimsIdentity identity =
            new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        ClaimsPrincipal principal = new(identity);

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);
    }

    private static string SafeReturnUrl(string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl)
            || !returnUrl.StartsWith("/", StringComparison.Ordinal)
            || returnUrl.StartsWith("//", StringComparison.Ordinal))
        {
            return "/";
        }

        return returnUrl;
    }
}