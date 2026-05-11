namespace MathTest.Web.Api;

internal sealed class IntegrationApiOptions
{
    public const string SectionName = "Integration";

    /// <summary>Shared secret for <c>X-Api-Key</c> on <c>/integration/v1/*</c>. Leave empty to disable the integration API (503).</summary>
    public string? ApiKey { get; set; }
}
