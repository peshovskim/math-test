using System.Security.Claims;
using MathTest.Application.ExamResults;
using MathTest.Application.Queries.TeacherExams;
using MathTest.Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace MathTest.Web.Api;

internal static class TeacherExamApiEndpoints
{
    public static WebApplication MapTeacherExamEndpoints(this WebApplication app)
    {
        RouteGroupBuilder api = app.MapGroup("/api/teacher")
            .RequireAuthorization(new AuthorizeAttribute { Roles = RoleNames.Teacher });

        api.MapGet("/exams", GetTeacherExamsAsync);
        api.MapPost("/exams/batch-xml", ExamXmlBatchUpload.HandleAsync).DisableAntiforgery();

        return app;
    }

    private static async Task<IResult> GetTeacherExamsAsync(
        ClaimsPrincipal user,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        string? idValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(idValue, out int teacherUserId))
        {
            return TypedResults.Unauthorized();
        }

        IReadOnlyList<TeacherExamListItem> exams =
            await mediator.Send(new GetTeacherExamsQuery(teacherUserId), cancellationToken);

        return TypedResults.Ok(exams);
    }
}
