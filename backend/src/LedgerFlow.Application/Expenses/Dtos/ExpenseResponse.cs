namespace LedgerFlow.Application.Expenses.Dtos;

public sealed class ExpenseResponse
{
    public Guid Id { get; init; }

    public string Vendor { get; init; } =
        string.Empty;

    public string Description { get; init; } =
        string.Empty;

    public string Category { get; init; } =
        string.Empty;

    public decimal Amount { get; init; }

    public DateTime ExpenseDate { get; init; }

    public string? Reference { get; init; }

    public string? Notes { get; init; }

    public DateTime CreatedAtUtc { get; init; }
}