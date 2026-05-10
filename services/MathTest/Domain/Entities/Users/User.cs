namespace MathTest.Domain.Entities.Users;

public sealed class User
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public byte[] PasswordHash { get; set; } = [];

    public byte[] Salt { get; set; } = [];

    public ICollection<UserRole> UserRoles { get; } = new List<UserRole>();
}
