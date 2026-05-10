using System.Data;
using System.Globalization;
using MathTest.MathEngine.Interfaces;
using MathTest.MathEngine.Models;

namespace MathTest.MathEngine.Services;

public sealed class MathEvaluator : IMathEvaluator
{
    public EvaluationResult Evaluate(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            return new EvaluationResult
            {
                Success = false,
                Error = "Empty expression.",
            };
        }

        try
        {
            object? computed = new DataTable().Compute(expression, null);

            if (computed is null or DBNull)
            {
                return new EvaluationResult
                {
                    Success = false,
                    Error = "No result.",
                };
            }

            double value = Convert.ToDouble(computed, CultureInfo.InvariantCulture);

            return new EvaluationResult
            {
                Success = true,
                Result = value,
            };
        }
        catch (Exception ex)
        {
            return new EvaluationResult
            {
                Success = false,
                Error = ex.Message,
            };
        }
    }
}
