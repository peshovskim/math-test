namespace MathTest.Application.Models;

public sealed class ExamProcessingResult
{
    public ParsedExamBatch Parsed { get; set; } = null!;

    public List<GradedTaskOutcome> GradedTasks { get; set; } = [];
}
