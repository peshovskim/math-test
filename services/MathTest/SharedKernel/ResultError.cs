namespace SharedKernel;

public sealed record ResultError
{
    public ResultType Type { get; set; }

    public string Code { get; set; }

    public string Message { get; set; }

    public ResultError(ResultType type, string code, string message)
    {
        Type = type;
        Code = code;
        Message = message;
    }
}