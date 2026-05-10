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

            ParsedExamBatch batch = new()
            {
                TeacherExternalId =
                    teacherElement.Attribute("ID")?.Value
                    ?? teacherElement.Attribute("id")?.Value
                    ?? string.Empty,
            };

            XElement? studentsElement = teacherElement.Element("Students");

            if (studentsElement is null)
            {
                return Result<ParsedExamBatch>.Invalid(
                    ResultCodes.Validation,
                    "Invalid XML. Missing students element.");
            }

            foreach (XElement studentElement in studentsElement.Elements("Student"))
            {
                ParsedStudentExam parsedStudentExam = new()
                {
                    StudentExternalId =
                        studentElement.Attribute("ID")?.Value
                        ?? studentElement.Attribute("id")?.Value
                        ?? string.Empty,
                };

                XElement? examElement = studentElement.Element("Exam");

                if (examElement is null)
                {
                    continue;
                }

                parsedStudentExam.ExamExternalId =
                    examElement.Attribute("Id")?.Value
                    ?? examElement.Attribute("id")?.Value
                    ?? string.Empty;

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

                    if (!int.TryParse(taskElement.Attribute("id")?.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int taskOrder))
                    {
                        taskOrder = 0;
                    }

                    parsedStudentExam.Tasks.Add(
                        new ParsedTask
                        {
                            TaskOrder = taskOrder,
                            Expression = expression,
                            StudentAnswer = studentAnswer,
                        });
                }

                batch.StudentExams.Add(parsedStudentExam);
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
}
