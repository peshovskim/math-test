namespace MathTest.Domain.Entities.Users;

public sealed class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<UserRole> UserRoles { get; } = new List<UserRole>();
}
