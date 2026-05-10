using MathTest.Domain.Entities.Exams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MathTest.Infrastructure.Persistence.Configurations;

public sealed class ExamTaskConfiguration : IEntityTypeConfiguration<ExamTask>
{
    public void Configure(EntityTypeBuilder<ExamTask> builder)
    {
        builder.ToTable("ExamTask", "dbo");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Expression).IsRequired();

        builder.Property(t => t.StudentAnswer).IsRequired();

        builder.Property(t => t.CorrectAnswer).IsRequired(false);
    }
}
