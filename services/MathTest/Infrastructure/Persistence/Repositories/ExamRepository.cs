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

    public Task<bool> ExistsByTeacherStudentAndExamExternalIdsAsync(
        string teacherExternalId,
        string examExternalId,
        string studentExternalId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Exams.AnyAsync(
            e => e.ExternalTeacherId == teacherExternalId
                 && e.ExternalId == examExternalId
                 && e.ExternalStudentId == studentExternalId,
            cancellationToken);
    }
}
