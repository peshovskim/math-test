using MathTest.Domain.Entities.Exams;

namespace MathTest.Application.Interfaces;

public interface IExamRepository
{
    Task AddAsync(Exam exam, CancellationToken cancellationToken = default);
}
