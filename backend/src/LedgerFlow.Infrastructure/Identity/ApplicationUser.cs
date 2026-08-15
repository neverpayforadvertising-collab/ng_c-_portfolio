using Microsoft.AspNetCore.Identity;

namespace LedgerFlow.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }
}