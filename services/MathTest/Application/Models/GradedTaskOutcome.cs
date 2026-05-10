namespace MathTest.Application.Models;

public sealed class GradedTaskOutcome
{
    public string StudentExternalId { get; set; } = string.Empty;

    public string ExamExternalId { get; set; } = string.Empty;

    public int TaskOrder { get; set; }

    public string Expression { get; set; } = string.Empty;

    public double StudentAnswer { get; set; }

    public double? CorrectAnswer { get; set; }

    public bool EvaluationSucceeded { get; set; }

    public string? EvaluationError { get; set; }

    public bool IsCorrect { get; set; }
}
