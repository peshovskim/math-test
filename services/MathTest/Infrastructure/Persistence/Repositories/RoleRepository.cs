using Microsoft.EntityFrameworkCore;
using MathTest.Application.Identity.Repositories;
using MathTest.Domain.Entities.Users;

namespace MathTest.Infrastructure.Persistence.Repositories;

public sealed class RoleRepository(AppDbContext dbContext) : IRoleRepository
{
    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        string normalizedName = name.Trim();
        return await dbContext.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(role => role.Name == normalizedName, cancellationToken);
    }
}
