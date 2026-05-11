namespace MathTest.Application.ExamResults;

public sealed class StudentExamTaskItem
{
    public int TaskId { get; init; }

    public string ExternalId { get; init; } = string.Empty;

    public string Expression { get; init; } = string.Empty;

    public double StudentAnswer { get; init; }

    public double? CorrectAnswer { get; init; }

    public bool IsCorrect { get; init; }
}

public sealed class StudentExamListItem
{
    public int ExamId { get; init; }

    public string ExamExternalId { get; init; } = string.Empty;

    public string FileName { get; init; } = string.Empty;

    public double Score { get; init; }

    public IReadOnlyList<StudentExamTaskItem> Tasks { get; init; } = Array.Empty<StudentExamTaskItem>();
}
