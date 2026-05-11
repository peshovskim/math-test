using static MathTest.Web.Api.ApiEndpointResults;

using MathTest.Application.Commands.ProcessExamXml;
using MathTest.Application.Models;
using MathTest.Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using SharedKernel;

namespace MathTest.Web.Api;

internal static class TeacherExamApiEndpoints
{
    public static WebApplication MapTeacherExamEndpoints(this WebApplication app)
    {
        RouteGroupBuilder api = app.MapGroup("/api/teacher")
            .RequireAuthorization(new AuthorizeAttribute { Roles = RoleNames.Teacher });

        api.MapPost("/exams/batch-xml", UploadExamXmlAsync).DisableAntiforgery();

        return app;
    }

    private static async Task<IResult> UploadExamXmlAsync(
        IFormFile? file,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return TypedResults.BadRequest();
        }

        await using Stream xmlStream = file.OpenReadStream();

        Result<ExamProcessingResult> result = await mediator.Send(
            new ProcessExamXmlCommand(
                xmlStream,
                file.FileName ?? string.Empty),
            cancellationToken);

        return OkOrError(result);
    }
}
