namespace MathTest.MathEngine.Models;

public sealed class EvaluationResult
{
    public bool Success { get; init; }

    public double? Result { get; init; }

    public string? Error { get; init; }
}
