using MathTest.Application.Common.Abstractions;
using MathTest.Application.Identity.Repositories;
using MathTest.Application.Interfaces;
using MathTest.Application.Models;
using MathTest.Domain.Entities.Exams;
using MathTest.Domain.Entities.Users;
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
    IMathEvaluator mathEvaluator,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    IExamRepository examRepository)
    : IRequestHandler<ProcessExamXmlCommand, Result<ExamProcessingResult>>
{
    public async Task<Result<ExamProcessingResult>> Handle(ProcessExamXmlCommand command, CancellationToken cancellationToken)
    {
        Result<MemoryStream> bufferResult = await BufferXmlAsync(command.XmlStream, cancellationToken);

        if (!bufferResult.IsSuccess)
        {
            return Result.FromError<MemoryStream, ExamProcessingResult>(bufferResult);
        }

        using (MemoryStream buffer = bufferResult.Value!)
        {
            Result<ParsedExamBatch> parseResult = parser.Parse(buffer);

            if (parseResult.IsFailure)
            {
                return Result.FromError<ParsedExamBatch, ExamProcessingResult>(parseResult);
            }

            ParsedExamBatch batch = parseResult.Value!;

            var graded = new List<GradedTask>();

            foreach (ParsedExam exam in batch.Exams)
            {
                foreach (ParsedExamTask task in exam.Tasks)
                {
                    EvaluationResult evaluation = mathEvaluator.Evaluate(task.Expression);

                    GradedTask row = new()
                    {
                        StudentExternalId = exam.StudentExternalId,
                        ExamExternalId = exam.ExamExternalId,
                        TaskExternalId = task.ExternalId,
                        Expression = task.Expression,
                        StudentAnswer = task.StudentAnswer,
                        EvaluationError = evaluation.Error,
                        CorrectAnswer = evaluation.Result,
                        IsCorrect = evaluation.Success
                            && evaluation.Result.HasValue
                            && evaluation.Result.Value == task.StudentAnswer,
                    };

                    graded.Add(row);
                }
            }

            ExamProcessingResult processingResult = new()
            {
                Parsed = batch,
                GradedTasks = graded,
            };

            try
            {
                await StageImportedExamsAsync(
                    processingResult,
                    command.FileName,
                    userRepository,
                    examRepository,
                    cancellationToken);

                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                return Result<ExamProcessingResult>.InternalError(
                    ResultCodes.InternalError,
                    $"Failed to save exam import: {ex.Message}");
            }

            return Result<ExamProcessingResult>.Success(processingResult);
        }
    }

    private static async Task StageImportedExamsAsync(
        ExamProcessingResult result,
        string fileName,
        IUserRepository users,
        IExamRepository exams,
        CancellationToken cancellationToken)
    {
        User? teacher = await users.GetByExternalIdAsync(result.Parsed.TeacherExternalId, cancellationToken);

        foreach (ParsedExam parsedExam in result.Parsed.Exams)
        {
            User? student = await users.GetByExternalIdAsync(parsedExam.StudentExternalId, cancellationToken);

            List<GradedTask> gradedForStudent = result.GradedTasks
                .Where(g => g.StudentExternalId == parsedExam.StudentExternalId &&
                            g.ExamExternalId == parsedExam.ExamExternalId)
                .ToList();

            double score = ComputeExamScore(gradedForStudent);

            Exam exam = new()
            {
                StudentUserId = student?.Id,
                TeacherUserId = teacher?.Id,
                FileName = fileName,
                ExternalId = parsedExam.ExamExternalId,
                ExternalStudentId = parsedExam.StudentExternalId,
                ExternalTeacherId = result.Parsed.TeacherExternalId,
                Score = score,
            };

            foreach (GradedTask g in gradedForStudent)
            {
                exam.ExamTasks.Add(
                    new ExamTask
                    {
                        ExternalId = g.TaskExternalId ?? string.Empty,
                        Expression = g.Expression,
                        StudentAnswer = g.StudentAnswer,
                        CorrectAnswer = g.CorrectAnswer,
                        IsCorrect = g.IsCorrect,
                    });
            }

            await exams.AddAsync(exam, cancellationToken);
        }
    }

    private static double ComputeExamScore(IReadOnlyList<GradedTask> gradedTasks)
    {
        int total = gradedTasks.Count;

        if (total == 0)
        {
            return 0;
        }

        int correct = gradedTasks.Count(static g => g.IsCorrect);

        return 100.0 * correct / total;
    }

    private static async Task<Result<MemoryStream>> BufferXmlAsync(Stream? xmlStream, CancellationToken cancellationToken)
    {
        if (xmlStream is null)
        {
            return Result<MemoryStream>.Invalid(ResultCodes.Validation, "No XML stream.");
        }

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
