using MathTest.Domain.Entities.Exams;
using MathTest.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MathTest.Infrastructure.Persistence.Configurations;

public sealed class ExamConfiguration : IEntityTypeConfiguration<Exam>
{
    public void Configure(EntityTypeBuilder<Exam> builder)
    {
        builder.ToTable("Exams", "dbo");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.FileName).HasMaxLength(512).IsRequired();

        builder.Property(e => e.ExamExternalId).HasMaxLength(256).IsRequired();

        builder.Property(e => e.Score).IsRequired();

        builder.HasOne(e => e.StudentUser)
            .WithMany()
            .HasForeignKey(e => e.StudentUserId)
            .IsRequired(false);

        builder.HasOne(e => e.TeacherUser)
            .WithMany()
            .HasForeignKey(e => e.TeacherUserId)
            .IsRequired(false);

        builder.HasMany(e => e.ExamTasks)
            .WithOne(t => t.Exam)
            .HasForeignKey(t => t.ExamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
