using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MathTest.Domain.Entities.Users;

namespace MathTest.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Role", "dbo");

        builder.HasKey(role => role.Id);

        builder.Property(role => role.Name)
            .HasColumnType("nvarchar(128)")
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(role => role.Name).IsUnique();
    }
}
