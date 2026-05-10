using MathTest.Application.Models;
using SharedKernel;

namespace MathTest.Application.Interfaces;

public interface IXmlExamParser
{
    Result<ParsedExamBatch> Parse(Stream xmlStream);
}
