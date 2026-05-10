using MathTest.Domain.Entities.Users;

namespace MathTest.Application.Identity.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
