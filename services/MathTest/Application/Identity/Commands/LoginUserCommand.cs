using MediatR;
using MathTest.Application.Identity.Repositories;
using MathTest.Application.Identity.Requests;
using MathTest.Application.Identity.Responses;
using MathTest.Application.Identity.Interfaces;
using MathTest.Domain.Entities.Users;
using SharedKernel;
using SharedKernel.Cqrs;

namespace MathTest.Application.Identity.Commands;

public sealed record LoginUserCommand(LoginUserRequest Request) : ICommand<Result<LoginResponse>>;

public sealed class LoginUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher)
    : IRequestHandler<LoginUserCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        LoginUserRequest request = command.Request;

        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Result<LoginResponse>.Invalid(ResultCodes.Validation, "Email and password are required.");
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        User? user = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null)
        {
            return Result<LoginResponse>.Unauthorized(ResultCodes.Unauthorized, "Invalid email or password.");
        }

        if (!passwordHasher.VerifyPassword(request.Password, user.PasswordHash, user.Salt))
        {
            return Result<LoginResponse>.Unauthorized(ResultCodes.Unauthorized, "Invalid email or password.");
        }

        IReadOnlyList<string> roleNames = await userRepository.GetRoleNamesAsync(user.Id, cancellationToken);

        return Result<LoginResponse>.Success(
            new LoginResponse
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleNames = roleNames,
            });
    }
}
