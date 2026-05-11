namespace MathTest.Application.ExamResults;

public interface ITeacherExamResultsQuery
{
    Task<IReadOnlyList<TeacherExamListItem>> GetExamsForTeacherAsync(
        int teacherUserId,
        CancellationToken cancellationToken = default);
}
