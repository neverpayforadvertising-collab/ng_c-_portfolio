namespace LedgerFlow.Application.Reports.Dtos;

public sealed class ExpenseReportResponse
{
    public DateTime FromDate { get; init; }

    public DateTime ToDate { get; init; }

    public decimal TotalExpenses { get; init; }

    public int ExpenseCount { get; init; }

    public decimal AverageExpense { get; init; }

    public decimal LargestExpense { get; init; }

    public IReadOnlyList<CategoryExpenseSummary>
        Categories { get; init; } =
            [];

    public IReadOnlyList<MonthlyExpenseSummary>
        MonthlyTrend { get; init; } =
            [];
}

public sealed class CategoryExpenseSummary
{
    public string Category { get; init; } =
        string.Empty;

    public decimal Amount { get; init; }

    public int Count { get; init; }
}

public sealed class MonthlyExpenseSummary
{
    public int Year { get; init; }

    public int Month { get; init; }

    public decimal Amount { get; init; }

    public int Count { get; init; }
}