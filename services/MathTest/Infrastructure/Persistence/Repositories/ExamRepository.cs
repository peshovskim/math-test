using MathTest.Application.Interfaces;
using MathTest.Domain.Entities.Exams;
using Microsoft.EntityFrameworkCore;

namespace MathTest.Infrastructure.Persistence.Repositories;

public sealed class ExamRepository(AppDbContext dbContext) : IExamRepository
{
    public async Task AddAsync(Exam exam, CancellationToken cancellationToken = default)
    {
        await dbContext.Exams.AddAsync(exam, cancellationToken);
    }
}
