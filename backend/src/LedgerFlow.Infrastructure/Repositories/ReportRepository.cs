using LedgerFlow.Application.Reports.Dtos;
using LedgerFlow.Application.Reports.Interfaces;
using LedgerFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LedgerFlow.Infrastructure.Repositories;

public sealed class ReportRepository
    : IReportRepository
{
    private readonly AppDbContext
        _dbContext;

    public ReportRepository(
        AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ExpenseReportResponse>
        GetExpenseReportAsync(
            DateTime fromDate,
            DateTime toDate,
            CancellationToken cancellationToken = default)
    {
        var toExclusive =
            toDate.Date.AddDays(1);

        var query =
            _dbContext.Expenses
                .AsNoTracking()
                .Where(x =>
                    !x.IsArchived &&
                    x.ExpenseDate >= fromDate.Date &&
                    x.ExpenseDate < toExclusive);

        var totalExpenses =
            await query
                .Select(x => (decimal?)x.Amount)
                .SumAsync(cancellationToken)
            ?? 0m;

        var expenseCount =
            await query.CountAsync(
                cancellationToken);

        var averageExpense =
            expenseCount == 0
                ? 0m
                : await query.AverageAsync(
                    x => x.Amount,
                    cancellationToken);

        var largestExpense =
            expenseCount == 0
                ? 0m
                : await query.MaxAsync(
                    x => x.Amount,
                    cancellationToken);

        var categories =
            await query
                .GroupBy(x => x.Category)
                .Select(group =>
                    new CategoryExpenseSummary
                    {
                        Category = group.Key,

                        Amount =
                            group.Sum(x => x.Amount),

                        Count =
                            group.Count()
                    })
                .OrderByDescending(
                    x => x.Amount)
                .ToListAsync(
                    cancellationToken);

        var monthlyTrend =
            await query
                .GroupBy(x => new
                {
                    x.ExpenseDate.Year,
                    x.ExpenseDate.Month
                })
                .Select(group =>
                    new MonthlyExpenseSummary
                    {
                        Year =
                            group.Key.Year,

                        Month =
                            group.Key.Month,

                        Amount =
                            group.Sum(
                                x => x.Amount),

                        Count =
                            group.Count()
                    })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync(
                    cancellationToken);

        return new ExpenseReportResponse
        {
            FromDate = fromDate.Date,
            ToDate = toDate.Date,

            TotalExpenses =
                totalExpenses,

            ExpenseCount =
                expenseCount,

            AverageExpense =
                averageExpense,

            LargestExpense =
                largestExpense,

            Categories =
                categories,

            MonthlyTrend =
                monthlyTrend
        };
    }
}