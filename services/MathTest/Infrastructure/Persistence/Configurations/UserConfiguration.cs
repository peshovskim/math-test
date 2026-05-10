using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MathTest.Domain.Entities.Users;

namespace MathTest.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User", "dbo");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.FirstName).HasColumnType("nvarchar(256)").IsRequired(false);

        builder.Property(user => user.LastName).HasColumnType("nvarchar(256)").IsRequired(false);

        builder.Property(user => user.Email).HasColumnType("nvarchar(256)").IsRequired();

        builder.Property(u => u.PasswordHash).HasColumnType("varbinary(max)").IsRequired();

        builder.Property(u => u.Salt).HasColumnType("varbinary(max)").IsRequired();
    }
}
