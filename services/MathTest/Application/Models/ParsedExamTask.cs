namespace MathTest.Application.Models;

public sealed class ParsedExamTask
{
    public string? ExternalId { get; set; }

    public string Expression { get; set; } = string.Empty;

    public double StudentAnswer { get; set; }
}
