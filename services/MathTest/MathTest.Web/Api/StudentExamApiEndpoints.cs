using System.Security.Claims;
using MathTest.Application.ExamResults;
using MathTest.Application.Queries.StudentExams;
using MathTest.Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace MathTest.Web.Api;

internal static class StudentExamApiEndpoints
{
    public static WebApplication MapStudentExamEndpoints(this WebApplication app)
    {
        RouteGroupBuilder api = app.MapGroup("/api/student")
            .RequireAuthorization(new AuthorizeAttribute { Roles = RoleNames.Student });

        api.MapGet("/exams", GetMyExamsAsync);

        return app;
    }

    private static async Task<IResult> GetMyExamsAsync(
        ClaimsPrincipal user,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        string? idValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(idValue, out int studentUserId))
        {
            return TypedResults.Unauthorized();
        }

        IReadOnlyList<StudentExamListItem> exams =
            await mediator.Send(new GetStudentExamsQuery(studentUserId), cancellationToken);

        return TypedResults.Ok(exams);
    }
}
