using MathTest.Application.ExamResults;
using MediatR;
using SharedKernel.Cqrs;

namespace MathTest.Application.Queries.StudentExams;

public sealed record GetStudentExamsQuery(int StudentUserId)
    : IQuery<IReadOnlyList<StudentExamListItem>>;

public sealed class GetStudentExamsQueryHandler(IStudentExamResultsQuery examResults)
    : IRequestHandler<GetStudentExamsQuery, IReadOnlyList<StudentExamListItem>>
{
    public Task<IReadOnlyList<StudentExamListItem>> Handle(
        GetStudentExamsQuery request,
        CancellationToken cancellationToken) =>
        examResults.GetExamsForStudentAsync(request.StudentUserId, cancellationToken);
}
