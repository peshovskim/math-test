using static MathTest.Web.Api.ApiEndpointResults;

using MathTest.Application.Commands.ProcessExamXml;
using MathTest.Application.Models;
using MediatR;
using SharedKernel;

namespace MathTest.Web.Api;

internal static class TeacherExamEndpoints
{
    public static WebApplication MapTeacherExamEndpoints(this WebApplication app)
    {
        RouteGroupBuilder api = app.MapGroup("/api/teacher");

        api.MapPost("/exams/batch-xml", UploadBatchXmlAsync).DisableAntiforgery();

        return app;
    }

    private static async Task<IResult> UploadBatchXmlAsync(
        IFormFile? file,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return TypedResults.BadRequest();
        }

        await using MemoryStream xmlStream = new();
        await file.CopyToAsync(xmlStream, cancellationToken);
        xmlStream.Position = 0;

        string fileName = file.FileName ?? string.Empty;

        Result<ExamProcessingResult> result =
            await mediator.Send(new ProcessExamXmlCommand(xmlStream, fileName), cancellationToken);

        return OkOrError(result);
    }
}
