using SharedKernel;

namespace MathTest.Web.Api;

internal static class ApiEndpointResults
{
    public static IResult OkOrError<T>(Result<T> result)
    {
        if (!result.IsSuccess)
        {
            return Fail(result);
        }

        return TypedResults.Ok(result.Value);
    }

    public static IResult OkOrError(Result result)
    {
        if (!result.IsSuccess)
        {
            return Fail(result);
        }

        return TypedResults.Ok();
    }

    private static IResult Fail(Result failed)
    {
        ResultError error = failed.Error!;

        return TypedResults.Json(
            new { code = error.Code, message = error.Message },
            statusCode: StatusFor(error.Type));
    }

    private static int StatusFor(ResultType type) =>
        type switch
        {
            ResultType.Invalid => StatusCodes.Status400BadRequest,
            ResultType.NotFound => StatusCodes.Status404NotFound,
            ResultType.Conflicted => StatusCodes.Status409Conflict,
            ResultType.Forbidden => StatusCodes.Status403Forbidden,
            ResultType.Unauthorized => StatusCodes.Status401Unauthorized,
            ResultType.InternalError => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError,
        };
}
