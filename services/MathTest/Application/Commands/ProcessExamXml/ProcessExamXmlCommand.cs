using MathTest.Application.Interfaces;
using MathTest.Application.Models;
using MathTest.MathEngine.Interfaces;
using MathTest.MathEngine.Models;
using MediatR;
using SharedKernel;
using SharedKernel.Cqrs;

namespace MathTest.Application.Commands.ProcessExamXml;

public sealed record ProcessExamXmlCommand(Stream XmlStream, string FileName)
    : ICommand<Result<ExamProcessingResult>>;

public sealed class ProcessExamXmlCommandHandler(
    IXmlExamParser parser,
    IMathEvaluator mathEvaluator)
    : IRequestHandler<ProcessExamXmlCommand, Result<ExamProcessingResult>>
{
    public async Task<Result<ExamProcessingResult>> Handle(ProcessExamXmlCommand command, CancellationToken cancellationToken)
    {
        Result<MemoryStream> bufferResult = await BufferXmlAsync(command.XmlStream, cancellationToken);

        if (!bufferResult.IsSuccess)
        {
            return Result.FromError<MemoryStream, ExamProcessingResult>(bufferResult);
        }

        await using MemoryStream buffer = bufferResult.Value!;

        Result<ParsedExamBatch> parseResult = parser.Parse(buffer);

        if (parseResult.IsFailure)
        {
            return Result.FromError<ParsedExamBatch, ExamProcessingResult>(parseResult);
        }

        ParsedExamBatch batch = parseResult.Value!;

        var graded = new List<GradedTaskOutcome>();

        foreach (ParsedStudentExam studentExam in batch.StudentExams)
        {
            foreach (ParsedTask task in studentExam.Tasks)
            {
                EvaluationResult evaluation = mathEvaluator.Evaluate(task.Expression);

                GradedTaskOutcome row = new()
                {
                    StudentExternalId = studentExam.StudentExternalId,
                    ExamExternalId = studentExam.ExamExternalId,
                    TaskOrder = task.TaskOrder,
                    Expression = task.Expression,
                    StudentAnswer = task.StudentAnswer,
                    EvaluationSucceeded = evaluation.Success,
                    EvaluationError = evaluation.Error,
                    CorrectAnswer = evaluation.Result,
                    IsCorrect = evaluation.Success
                        && evaluation.Result.HasValue
                        && evaluation.Result.Value == task.StudentAnswer,
                };

                graded.Add(row);
            }
        }

        return Result<ExamProcessingResult>.Success(
            new ExamProcessingResult
            {
                Parsed = batch,
                GradedTasks = graded,
            });
    }

    private static async Task<Result<MemoryStream>> BufferXmlAsync(Stream xmlStream, CancellationToken cancellationToken)
    {
        MemoryStream buffer = new();

        await xmlStream.CopyToAsync(buffer, cancellationToken);

        if (buffer.Length == 0)
        {
            return Result<MemoryStream>.Invalid(ResultCodes.Validation, "Empty XML stream.");
        }

        buffer.Position = 0;

        return Result<MemoryStream>.Success(buffer);
    }
}
