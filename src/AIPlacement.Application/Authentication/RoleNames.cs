namespace AIPlacement.Application.Authentication;

public static class RoleNames
{
    public const string Student = "Student";
    public const string Company = "Company";
    public const string Admin = "Admin";

    public static readonly IReadOnlySet<string> All =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Student,
            Company,
            Admin
        };
}
