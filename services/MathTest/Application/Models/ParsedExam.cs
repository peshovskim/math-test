namespace MathTest.Application.Models;

public sealed class ParsedExam
{
    public string StudentExternalId { get; set; } = string.Empty;

    public string ExamExternalId { get; set; } = string.Empty;

    public List<ParsedExamTask> Tasks { get; set; } = [];
}
