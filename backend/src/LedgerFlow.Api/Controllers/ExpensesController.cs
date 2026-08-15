using LedgerFlow.Api.Authorization;
using LedgerFlow.Application.Expenses.Dtos;
using LedgerFlow.Application.Expenses.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LedgerFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(
    Policy = AppPolicies.CanViewExpenses
)]
public sealed class ExpensesController
    : ControllerBase
{
    private readonly IExpenseService
        _expenseService;

    public ExpensesController(
        IExpenseService expenseService)
    {
        _expenseService =
            expenseService;
    }

    [HttpGet]
    public async Task<
        ActionResult<List<ExpenseResponse>>
    > GetAll(
        CancellationToken cancellationToken)
    {
        var expenses =
            await _expenseService.GetAllAsync(
                cancellationToken);

        return Ok(expenses);
    }

    [HttpGet("{id:guid}")]
    public async Task<
        ActionResult<ExpenseResponse>
    > GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var expense =
            await _expenseService.GetByIdAsync(
                id,
                cancellationToken);

        if (expense is null)
        {
            return NotFound();
        }

        return Ok(expense);
    }

    [HttpPost]
    [Authorize(
        Policy = AppPolicies.CanManageExpenses
    )]
    public async Task<
        ActionResult<ExpenseResponse>
    > Create(
        CreateExpenseRequest request,
        CancellationToken cancellationToken)
    {
        var expense =
            await _expenseService.CreateAsync(
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = expense.Id },
            expense);
    }
}