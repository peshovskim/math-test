namespace MathTest.Application.Identity.Responses;

public sealed class LoginResponse
{
    public required int UserId { get; init; }

    public required string Email { get; init; }

    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public required IReadOnlyList<string> RoleNames { get; init; }
}
