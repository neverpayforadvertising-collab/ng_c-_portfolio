using LedgerFlow.Application.Reports.Dtos;
using LedgerFlow.Application.Reports.Interfaces;

namespace LedgerFlow.Application.Reports.Services;

public sealed class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;

    public ReportService(
        IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public Task<ExpenseReportResponse> GetExpenseReportAsync(
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        if (fromDate.Date > toDate.Date)
        {
            throw new ArgumentException(
                "From date cannot be later than to date.");
        }

        return _reportRepository.GetExpenseReportAsync(
            fromDate.Date,
            toDate.Date,
            cancellationToken);
    }
}
