using MathTest.Domain.Entities.Users;
using Microsoft.AspNetCore.Authorization;

namespace MathTest.Web.Api;

internal static class TeacherExamApiEndpoints
{
    public static WebApplication MapTeacherExamEndpoints(this WebApplication app)
    {
        RouteGroupBuilder api = app.MapGroup("/api/teacher")
            .RequireAuthorization(new AuthorizeAttribute { Roles = RoleNames.Teacher });

        api.MapPost("/exams/batch-xml", ExamXmlBatchUpload.HandleAsync).DisableAntiforgery();

        return app;
    }
}
