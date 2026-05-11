namespace MathTest.Application.ExamResults;

public sealed class ExamTaskRow
{
    public int TaskId { get; init; }

    public string ExternalId { get; init; } = string.Empty;

    public string Expression { get; init; } = string.Empty;

    public double StudentAnswer { get; init; }

    public double? CorrectAnswer { get; init; }

    public bool IsCorrect { get; init; }
}
