using LedgerFlow.Api.Authorization;
using LedgerFlow.Application.Reports.Dtos;
using LedgerFlow.Application.Reports.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LedgerFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(
    Policy = AppPolicies.CanViewReports
)]
public sealed class ReportsController
    : ControllerBase
{
    private readonly IReportService
        _reportService;

    public ReportsController(
        IReportService reportService)
    {
        _reportService =
            reportService;
    }

    [HttpGet("expenses")]
    public async Task<
        ActionResult<ExpenseReportResponse>
    > GetExpenseReport(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken cancellationToken)
    {
        var toDate =
            (to ?? DateTime.UtcNow).Date;

        var fromDate =
            (
                from ??
                toDate
                    .AddMonths(-5)
                    .AddDays(
                        1 - toDate.Day)
            ).Date;

        if (fromDate > toDate)
        {
            return BadRequest(
                new ProblemDetails
                {
                    Title =
                        "Invalid report range",

                    Detail =
                        "The from date cannot be later than the to date."
                });
        }

        var report =
            await _reportService
                .GetExpenseReportAsync(
                    fromDate,
                    toDate,
                    cancellationToken);

        return Ok(report);
    }
}