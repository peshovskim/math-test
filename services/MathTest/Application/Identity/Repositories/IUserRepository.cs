using MathTest.Domain.Entities.Users;

namespace MathTest.Application.Identity.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<User?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default);

    Task AddAsync(User user, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetRoleNamesAsync(int userId, CancellationToken cancellationToken = default);
}
