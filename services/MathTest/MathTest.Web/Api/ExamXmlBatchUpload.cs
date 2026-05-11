using MathTest.Application.Commands.ProcessExamXml;
using MathTest.Application.Models;
using MediatR;
using SharedKernel;

namespace MathTest.Web.Api;

internal static class ExamXmlBatchUpload
{
    internal static async Task<IResult> HandleAsync(
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

        return ApiEndpointResults.OkOrError(result);
    }
}
