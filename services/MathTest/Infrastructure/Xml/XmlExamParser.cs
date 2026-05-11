using System.Globalization;
using System.Xml.Linq;
using MathTest.Application.Interfaces;
using MathTest.Application.Models;
using SharedKernel;

namespace MathTest.Infrastructure.Xml;

public sealed class XmlExamParser : IXmlExamParser
{
    public Result<ParsedExamBatch> Parse(Stream xmlStream)
    {
        try
        {
            XDocument document = XDocument.Load(xmlStream);

            XElement? teacherElement = document.Root;

            if (teacherElement is null)
            {
                return Result<ParsedExamBatch>.Invalid(
                    ResultCodes.Validation,
                    "Invalid XML. Missing teacher root element.");
            }

            string teacherExternalId = GetAttributeValueIgnoreCase(teacherElement, "id") ?? string.Empty;

            if (teacherExternalId.Length == 0)
            {
                return Result<ParsedExamBatch>.Invalid(
                    ResultCodes.Validation,
                    "Invalid XML. Teacher element must have an ID attribute.");
            }

            ParsedExamBatch batch = new() { TeacherExternalId = teacherExternalId };

            XElement? studentsElement = teacherElement.Element("Students");

            if (studentsElement is null)
            {
                return Result<ParsedExamBatch>.Invalid(
                    ResultCodes.Validation,
                    "Invalid XML. Missing students element.");
            }

            foreach (XElement studentElement in studentsElement.Elements("Student"))
            {
                ParsedExam parsedExam = new()
                {
                    StudentExternalId = GetAttributeValueIgnoreCase(studentElement, "id") ?? string.Empty,
                };

                XElement? examElement = studentElement.Element("Exam");

                if (examElement is null)
                {
                    continue;
                }

                parsedExam.ExamExternalId = GetAttributeValueIgnoreCase(examElement, "id") ?? string.Empty;

                foreach (XElement taskElement in examElement.Elements("Task"))
                {
                    string rawTask = taskElement.Value;

                    if (string.IsNullOrWhiteSpace(rawTask))
                    {
                        continue;
                    }

                    string[] split = rawTask.Split('=');

                    if (split.Length != 2)
                    {
                        continue;
                    }

                    string expression = split[0].Trim();

                    string answerText = split[1].Trim();

                    if (!double.TryParse(answerText, NumberStyles.Float, CultureInfo.InvariantCulture, out double studentAnswer))
                    {
                        continue;
                    }

                    string? taskExternalId = GetAttributeValueIgnoreCase(taskElement, "id");

                    parsedExam.Tasks.Add(
                        new ParsedExamTask
                        {
                            ExternalId = taskExternalId,
                            Expression = expression,
                            StudentAnswer = studentAnswer,
                        });
                }

                batch.Exams.Add(parsedExam);
            }

            return Result<ParsedExamBatch>.Success(batch);
        }
        catch (Exception ex)
        {
            return Result<ParsedExamBatch>.Invalid(
                ResultCodes.Validation,
                $"Failed to parse XML. {ex.Message}");
        }
    }

    private static string? GetAttributeValueIgnoreCase(XElement element, string name)
    {
        return element
            .Attributes()
            .FirstOrDefault(a =>
                string.Equals(a.Name.LocalName, name, StringComparison.OrdinalIgnoreCase))
            ?.Value?
            .Trim();
    }
}
