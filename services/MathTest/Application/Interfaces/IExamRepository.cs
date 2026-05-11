using MathTest.Domain.Entities.Exams;

namespace MathTest.Application.Interfaces;

public interface IExamRepository
{
    Task AddAsync(Exam exam, CancellationToken cancellationToken = default);

    /// <summary>Returns true if the same teacher, student, and exam external ids already exist on an exam row.</summary>
    Task<bool> ExistsByTeacherStudentAndExamExternalIdsAsync(
        string teacherExternalId,
        string examExternalId,
        string studentExternalId,
        CancellationToken cancellationToken = default);
}
