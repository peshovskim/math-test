using MediatR;
using MathTest.Application.Common.Abstractions;
using MathTest.Application.ExamImport;
using MathTest.Application.Identity.Requests;
using MathTest.Application.Identity.Repositories;
using MathTest.Application.Identity.Interfaces;
using MathTest.Domain.Entities.Users;
using SharedKernel;
using SharedKernel.Cqrs;

namespace MathTest.Application.Identity.Commands;

public sealed record RegisterUserCommand(RegisterUserRequest Request) : ICommand<Result>;

public sealed class RegisterUserCommandHandler(
    IPasswordHasher passwordHasher,
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RegisterUserCommand, Result>
{
    public async Task<Result> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        if (string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.LastName) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            string.IsNullOrWhiteSpace(request.ExternalId))
        {
            return Result.Invalid(ResultCodes.Validation, "First name, last name, email, password, and external ID are required.");
        }

        string externalId = request.ExternalId.Trim();

        if (request.RegisterAsTeacher)
        {
            if (!PredefinedExternalIds.IsKnownTeacher(externalId))
            {
                return Result.Invalid(ResultCodes.Validation, "The external ID is not valid.");
            }
        }
        else
        {
            if (!PredefinedExternalIds.IsKnownStudent(externalId))
            {
                return Result.Invalid(ResultCodes.Validation, "The external ID is not valid.");
            }
        }

        User? existingByExternalId = await userRepository.GetByExternalIdAsync(externalId, cancellationToken);

        if (existingByExternalId is not null)
        {
            return Result.Conflicted(ResultCodes.Conflict, "This external ID is already registered.");
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        User? existingUser = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (existingUser is not null)
        {
            return Result.Conflicted(ResultCodes.Conflict, "A user with this email already exists.");
        }

        string roleName = request.RegisterAsTeacher ? RoleNames.Teacher : RoleNames.Student;

        Role? role = await roleRepository.GetByNameAsync(roleName, cancellationToken);

        if (role is null)
        {
            return Result.Invalid(ResultCodes.Validation, "Missing role row.");
        }

        (byte[]? hash, byte[]? salt) = passwordHasher.HashPassword(request.Password);

        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = normalizedEmail,
            ExternalId = externalId,
            PasswordHash = hash,
            Salt = salt,
        };

        user.UserRoles.Add(new UserRole { RoleId = role.Id });

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
