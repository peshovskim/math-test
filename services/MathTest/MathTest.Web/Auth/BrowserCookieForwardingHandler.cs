using Microsoft.AspNetCore.Http;

namespace MathTest.Web.Auth;

public sealed class BrowserCookieForwardingHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpContext? httpContext = httpContextAccessor.HttpContext;

        if (httpContext?.Request.Headers.TryGetValue("Cookie", out Microsoft.Extensions.Primitives.StringValues cookieHeader) == true)
        {
            request.Headers.TryAddWithoutValidation("Cookie", cookieHeader.ToArray());
        }

        return base.SendAsync(request, cancellationToken);
    }
}
