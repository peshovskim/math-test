using MathTest.MathEngine.Models;

namespace MathTest.MathEngine.Interfaces;

public interface IMathEvaluator
{
    EvaluationResult Evaluate(string expression);
}
