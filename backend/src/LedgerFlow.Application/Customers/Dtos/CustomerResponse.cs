namespace LedgerFlow.Application.Customers.Dtos;

public sealed class CustomerResponse
{
    public Guid Id { get; init; }

    public string CompanyName { get; init; } = string.Empty;

    public string ContactName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string? Phone { get; init; }

    public string? City { get; init; }

    public string? State { get; init; }

    public string Country { get; init; } = string.Empty;

    public bool IsActive { get; init; }

    public DateTime CreatedAtUtc { get; init; }
}