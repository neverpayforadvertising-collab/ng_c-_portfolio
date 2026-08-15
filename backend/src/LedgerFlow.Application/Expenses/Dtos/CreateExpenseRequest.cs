using System.ComponentModel.DataAnnotations;

namespace LedgerFlow.Application.Expenses.Dtos;

public sealed class CreateExpenseRequest
{
    [Required]
    [MaxLength(200)]
    public string Vendor { get; init; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string Description { get; init; } = string.Empty;

    [Required]
    [MaxLength(80)]
    public string Category { get; init; } = string.Empty;

    [Range(
        typeof(decimal),
        "0.01",
        "9999999999999999.99"
    )]
    public decimal Amount { get; init; }

    public DateTime ExpenseDate { get; init; }

    [MaxLength(100)]
    public string? Reference { get; init; }

    [MaxLength(1000)]
    public string? Notes { get; init; }
}
