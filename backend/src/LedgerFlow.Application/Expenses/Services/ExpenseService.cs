
using LedgerFlow.Application.Expenses.Dtos;
using LedgerFlow.Application.Expenses.Interfaces;
using LedgerFlow.Domain.Entities;

namespace LedgerFlow.Application.Expenses.Services;

public sealed class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _expenseRepository;

    public ExpenseService(
        IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<List<ExpenseResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var expenses =
            await _expenseRepository.GetAllAsync(
                cancellationToken);

        return expenses
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<ExpenseResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var expense =
            await _expenseRepository.GetByIdAsync(
                id,
                cancellationToken);

        return expense is null
            ? null
            : MapToResponse(expense);
    }

    public async Task<ExpenseResponse> CreateAsync(
        CreateExpenseRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Amount <= 0)
        {
            throw new ArgumentException(
                "Expense amount must be greater than zero.");
        }

        if (request.ExpenseDate == default)
        {
            throw new ArgumentException(
                "Expense date is required.");
        }

        var now = DateTime.UtcNow;

        var expense = new Expense
        {
            Id = Guid.NewGuid(),

            Vendor =
                request.Vendor.Trim(),

            Description =
                request.Description.Trim(),

            Category =
                request.Category.Trim(),

            Amount =
                decimal.Round(
                    request.Amount,
                    2,
                    MidpointRounding.AwayFromZero),

            ExpenseDate =
                request.ExpenseDate.Date,

            Reference =
                Clean(request.Reference),

            Notes =
                Clean(request.Notes),

            IsArchived = false,

            CreatedAtUtc = now,

            UpdatedAtUtc = now
        };

        await _expenseRepository.AddAsync(
            expense,
            cancellationToken);

        return MapToResponse(expense);
    }

    private static string? Clean(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static ExpenseResponse MapToResponse(
        Expense expense)
    {
        return new ExpenseResponse
        {
            Id = expense.Id,

            Vendor = expense.Vendor,

            Description = expense.Description,

            Category = expense.Category,

            Amount = expense.Amount,

            ExpenseDate = expense.ExpenseDate,

            Reference = expense.Reference,

            Notes = expense.Notes,

            CreatedAtUtc = expense.CreatedAtUtc
        };
    }
}
