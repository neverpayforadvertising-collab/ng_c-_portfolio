namespace LedgerFlow.Domain.Entities;

public sealed class Expense
{
    public Guid Id { get; set; }

    public string Vendor { get; set; } =
        string.Empty;

    public string Description { get; set; } =
        string.Empty;

    public string Category { get; set; } =
        string.Empty;

    public decimal Amount { get; set; }

    public DateTime ExpenseDate { get; set; }

    public string? Reference { get; set; }

    public string? Notes { get; set; }

    public bool IsArchived { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}