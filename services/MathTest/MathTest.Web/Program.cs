using MathTest.Application;
using MathTest.Infrastructure;
using MathTest.Web.Api;
using MathTest.Web.Components;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapAuthApi();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
