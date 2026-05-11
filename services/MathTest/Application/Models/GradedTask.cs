namespace MathTest.Application.Models;

public sealed class GradedTask
{
    public string StudentExternalId { get; set; } = string.Empty;

    public string ExamExternalId { get; set; } = string.Empty;

    public string? TaskExternalId { get; set; }

    public string Expression { get; set; } = string.Empty;

    public double StudentAnswer { get; set; }

    public double? CorrectAnswer { get; set; }

    public string? EvaluationError { get; set; }

    public bool IsCorrect { get; set; }
}
