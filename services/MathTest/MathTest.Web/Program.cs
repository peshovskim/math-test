using MathTest.Application;
using MathTest.Infrastructure;
using MathTest.MathEngine;
using MathTest.Web.Api;
using MathTest.Web.Auth;
using MathTest.Web.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddMathEngine();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = ".MathTest.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.LoginPath = "/login";
    });

builder.Services.AddAuthorization();

builder.Services
    .AddOptions<IntegrationApiOptions>()
    .Bind(builder.Configuration.GetSection(IntegrationApiOptions.SectionName))
    .Validate(o => string.IsNullOrWhiteSpace(o.ApiKey) || o.ApiKey.Length >= 8, "Integration:ApiKey must be at least 8 characters when set.")
    .ValidateOnStart();

builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
builder.Services.AddTransient<BrowserCookieForwardingHandler>();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped(sp =>
{
    NavigationManager navigationManager = sp.GetRequiredService<NavigationManager>();
    BrowserCookieForwardingHandler handler = sp.GetRequiredService<BrowserCookieForwardingHandler>();
    handler.InnerHandler = new HttpClientHandler();
    return new HttpClient(handler)
    {
        BaseAddress = new Uri(navigationManager.BaseUri),
    };
});

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapAuthApi();
app.MapTeacherExamEndpoints();
app.MapIntegrationEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
