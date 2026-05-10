using MathTest.Domain.Entities.Users;

namespace MathTest.Domain.Entities.Exams;

public sealed class Exam
{
    public int Id { get; set; }

    public int? StudentUserId { get; set; }

    public int? TeacherUserId { get; set; }

    public User? StudentUser { get; set; }

    public User? TeacherUser { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string ExamExternalId { get; set; } = string.Empty;

    public double Score { get; set; }

    public ICollection<ExamTask> ExamTasks { get; } = new List<ExamTask>();
}
