namespace MathTest.Application.ExamImport;

public static class PredefinedExternalIds
{
    private static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;

    public static readonly IReadOnlyList<string> TeacherExternalIds =
    [
        "T-10001",
        "T-10002",
        "T-10003",
    ];

    public static readonly IReadOnlyList<string> StudentExternalIds =
    [
        "S-20001",
        "S-20002",
        "S-20003",
        "S-20004",
        "S-20005",
    ];

    private static readonly HashSet<string> TeacherSet = new(TeacherExternalIds, Comparer);

    private static readonly HashSet<string> StudentSet = new(StudentExternalIds, Comparer);

    public static bool IsKnownTeacher(string? externalId) =>
        !string.IsNullOrWhiteSpace(externalId) && TeacherSet.Contains(externalId.Trim());

    public static bool IsKnownStudent(string? externalId) =>
        !string.IsNullOrWhiteSpace(externalId) && StudentSet.Contains(externalId.Trim());
}
