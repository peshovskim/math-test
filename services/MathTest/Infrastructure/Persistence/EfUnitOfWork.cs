using Microsoft.EntityFrameworkCore;
using MathTest.Application.Common.Abstractions;

namespace MathTest.Infrastructure.Persistence;

public sealed class EfUnitOfWork<TDbContext>(TDbContext dbContext) : IUnitOfWork
    where TDbContext : DbContext
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
