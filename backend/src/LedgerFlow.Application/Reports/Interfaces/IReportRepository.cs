using LedgerFlow.Application.Reports.Dtos;

namespace LedgerFlow.Application.Reports.Interfaces;

public interface IReportRepository
{
    Task<ExpenseReportResponse> GetExpenseReportAsync(
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);
}
