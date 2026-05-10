namespace MathTest.Application.Models;

public sealed class ParsedTask
{
    public int TaskOrder { get; set; }

    public string Expression { get; set; } = string.Empty;

    public double StudentAnswer { get; set; }
}
