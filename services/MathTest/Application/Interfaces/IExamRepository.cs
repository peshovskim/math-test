using MathTest.Domain.Entities.Exams;

namespace MathTest.Application.Interfaces;

public interface IExamRepository
{
    Task AddAsync(Exam exam, CancellationToken cancellationToken = default);

    /// <summary>Returns true if an exam row already exists for this XML exam id (<see cref="Exam.ExternalId"/>).</summary>
    Task<bool> ExistsByExamExternalIdAsync(string examExternalId, CancellationToken cancellationToken = default);
}
