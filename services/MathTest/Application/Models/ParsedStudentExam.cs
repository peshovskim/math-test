namespace MathTest.Application.Models;

public sealed class ParsedStudentExam
{
    public string StudentExternalId { get; set; } = string.Empty;

    public string ExamExternalId { get; set; } = string.Empty;

    public List<ParsedTask> Tasks { get; set; } = [];
}
