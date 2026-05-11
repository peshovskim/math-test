namespace MathTest.Application.ExamResults;

public sealed class TeacherExamListItem
{
    public int ExamId { get; init; }

    public string ExamExternalId { get; init; } = string.Empty;

    public string FileName { get; init; } = string.Empty;

    public string ExternalStudentId { get; init; } = string.Empty;

    public double Score { get; init; }

    public IReadOnlyList<ExamTaskRow> Tasks { get; init; } = Array.Empty<ExamTaskRow>();
}
