namespace MathTest.Infrastructure.Options;

public sealed class DatabaseOptions
{
    /// <summary>
    /// SQL Server connection string
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}
