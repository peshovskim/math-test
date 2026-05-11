namespace MathTest.Application.ExamResults;

public interface IStudentExamResultsQuery
{
    Task<IReadOnlyList<StudentExamListItem>> GetExamsForStudentAsync(
        int studentUserId,
        CancellationToken cancellationToken = default);
}
