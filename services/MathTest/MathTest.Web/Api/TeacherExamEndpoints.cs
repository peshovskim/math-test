namespace MathTest.Web.Api;

internal static class TeacherExamEndpoints
{
    public static WebApplication MapTeacherExamEndpoints(this WebApplication app)
    {
        RouteGroupBuilder api = app.MapGroup("/api/teacher");

        api.MapPost("/exams/batch-xml", UploadBatchXmlAsync).DisableAntiforgery();

        return app;
    }

    private static IResult UploadBatchXmlAsync(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            return TypedResults.BadRequest();
        }

        return TypedResults.Ok();
    }
}
