using MathTest.Domain.Entities.Exams;

namespace MathTest.Application.Interfaces;

public interface IExamRepository
{
    Task AddAsync(Exam exam, CancellationToken cancellationToken = default);

    Task<bool> ExistsByExamAndStudentExternalIdsAsync(
        string examExternalId,
        string studentExternalId,
        CancellationToken cancellationToken = default);
}
