using MathTest.Application.ExamResults;
using MathTest.Domain.Entities.Exams;
using Microsoft.EntityFrameworkCore;

namespace MathTest.Infrastructure.Persistence.Queries;

public sealed class StudentExamResultsQuery(AppDbContext dbContext) : IStudentExamResultsQuery
{
    public async Task<IReadOnlyList<StudentExamListItem>> GetExamsForStudentAsync(
        int studentUserId,
        CancellationToken cancellationToken = default)
    {
        List<Exam> exams = await dbContext.Exams
            .AsNoTracking()
            .AsSplitQuery()
            .Include(e => e.ExamTasks)
            .Where(e => e.StudentUserId == studentUserId)
            .OrderByDescending(e => e.Id)
            .ToListAsync(cancellationToken);

        List<StudentExamListItem> result = new(capacity: exams.Count);

        foreach (Exam exam in exams)
        {
            IReadOnlyList<StudentExamTaskItem> tasks = exam.ExamTasks
                .OrderBy(t => t.Id)
                .Select(t => new StudentExamTaskItem
                {
                    TaskId = t.Id,
                    ExternalId = t.ExternalId,
                    Expression = t.Expression,
                    StudentAnswer = t.StudentAnswer,
                    CorrectAnswer = t.CorrectAnswer,
                    IsCorrect = t.IsCorrect,
                })
                .ToList();

            result.Add(
                new StudentExamListItem
                {
                    ExamId = exam.Id,
                    ExamExternalId = exam.ExternalId,
                    FileName = exam.FileName,
                    Score = exam.Score,
                    Tasks = tasks,
                });
        }

        return result;
    }
}
