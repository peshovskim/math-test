namespace MathTest.Domain.Entities.Exams;

public sealed class ExamTask
{
    public int Id { get; set; }

    public int ExamId { get; set; }

    public string ExternalId { get; set; } = string.Empty;

    public string Expression { get; set; } = string.Empty;

    public double StudentAnswer { get; set; }

    public double? CorrectAnswer { get; set; }

    public bool IsCorrect { get; set; }

    public Exam? Exam { get; set; }
}
