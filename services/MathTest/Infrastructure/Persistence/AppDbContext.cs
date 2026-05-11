using Microsoft.EntityFrameworkCore;
using MathTest.Domain.Entities.Exams;
using MathTest.Domain.Entities.Users;

namespace MathTest.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<Exam> Exams => Set<Exam>();

    public DbSet<ExamTask> ExamTasks => Set<ExamTask>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
