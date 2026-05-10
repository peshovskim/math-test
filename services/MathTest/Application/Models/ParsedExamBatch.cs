namespace MathTest.Application.Models;

public sealed class ParsedExamBatch
{
    public string TeacherExternalId { get; set; } = string.Empty;

    public List<ParsedStudentExam> StudentExams { get; set; } = [];
}
