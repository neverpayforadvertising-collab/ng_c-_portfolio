namespace LedgerFlow.Api.Authorization;

public static class AppRoles
{
    public const string Admin = "Admin";

    public const string Accountant = "Accountant";

    public const string Viewer = "Viewer";

    public static readonly string[] All =
    [
        Admin,
        Accountant,
        Viewer
    ];
}