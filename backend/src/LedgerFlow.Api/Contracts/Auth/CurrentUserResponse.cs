namespace LedgerFlow.Api.Contracts.Auth;

public sealed class CurrentUserResponse
{
    public string Id { get; init; } =
        string.Empty;

    public string Email { get; init; } =
        string.Empty;

    public string DisplayName { get; init; } =
        string.Empty;

    public IReadOnlyList<string> Roles {
        get;
        init;
    } = [];
}