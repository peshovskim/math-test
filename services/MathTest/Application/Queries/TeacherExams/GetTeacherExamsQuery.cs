using MathTest.Application.ExamResults;
using MediatR;
using SharedKernel.Cqrs;

namespace MathTest.Application.Queries.TeacherExams;

public sealed record GetTeacherExamsQuery(int TeacherUserId)
    : IQuery<IReadOnlyList<TeacherExamListItem>>;

public sealed class GetTeacherExamsQueryHandler(ITeacherExamResultsQuery examResults)
    : IRequestHandler<GetTeacherExamsQuery, IReadOnlyList<TeacherExamListItem>>
{
    public Task<IReadOnlyList<TeacherExamListItem>> Handle(
        GetTeacherExamsQuery request,
        CancellationToken cancellationToken) =>
        examResults.GetExamsForTeacherAsync(request.TeacherUserId, cancellationToken);
}
