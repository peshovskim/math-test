namespace MathTest.Application.Math.Abstractions;

/// <summary>
/// Processor boundary for third-party and application use: evaluates left-hand arithmetic expressions.
/// </summary>
public interface IArithmeticExpressionEvaluator
{
    decimal? TryEvaluate(string expression);
}
