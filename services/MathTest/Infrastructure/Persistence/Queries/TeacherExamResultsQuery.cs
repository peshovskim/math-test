using MathTest.Application.ExamResults;
using MathTest.Domain.Entities.Exams;
using Microsoft.EntityFrameworkCore;

namespace MathTest.Infrastructure.Persistence.Queries;

public sealed class TeacherExamResultsQuery(AppDbContext dbContext) : ITeacherExamResultsQuery
{
    public async Task<IReadOnlyList<TeacherExamListItem>> GetExamsForTeacherAsync(
        int teacherUserId,
        CancellationToken cancellationToken = default)
    {
        List<Exam> exams = await dbContext.Exams
            .AsNoTracking()
            .AsSplitQuery()
            .Include(e => e.ExamTasks)
            .Where(e => e.TeacherUserId == teacherUserId)
            .OrderByDescending(e => e.Id)
            .ToListAsync(cancellationToken);

        List<TeacherExamListItem> result = new(capacity: exams.Count);

        foreach (Exam exam in exams)
        {
            IReadOnlyList<ExamTaskRow> tasks = exam.ExamTasks
                .OrderBy(t => t.Id)
                .Select(t => new ExamTaskRow
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
                new TeacherExamListItem
                {
                    ExamId = exam.Id,
                    ExamExternalId = exam.ExternalId,
                    FileName = exam.FileName,
                    ExternalStudentId = exam.ExternalStudentId,
                    Score = exam.Score,
                    Tasks = tasks,
                });
        }

        return result;
    }
}
