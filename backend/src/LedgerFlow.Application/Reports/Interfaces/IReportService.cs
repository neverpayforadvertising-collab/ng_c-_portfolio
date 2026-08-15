using LedgerFlow.Application.Reports.Dtos;

namespace LedgerFlow.Application.Reports.Interfaces;

public interface IReportService
{
    Task<ExpenseReportResponse> GetExpenseReportAsync(
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);
}
