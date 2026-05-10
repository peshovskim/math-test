namespace MathTest.Application.Math.Abstractions;

public interface IArithmeticExpressionEvaluator
{
    decimal? TryEvaluate(string expression);
}
