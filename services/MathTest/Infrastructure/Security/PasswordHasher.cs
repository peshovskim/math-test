using System.Security.Cryptography;
using MathTest.Application.Identity.Interfaces;

namespace MathTest.Infrastructure.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    private const int Iterations = 100_000;
    private const int SaltSize = 16;
    private const int HashSize = 32;

    public (byte[] Hash, byte[] Salt) HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));
        }

        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        return (hash, salt);
    }

    public bool VerifyPassword(string password, byte[] hash, byte[] salt)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        if (hash is null || hash.Length == 0)
        {
            return false;
        }

        if (salt is null || salt.Length == 0)
        {
            return false;
        }

        var computed = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            hash.Length);

        return CryptographicOperations.FixedTimeEquals(computed, hash);
    }
}
