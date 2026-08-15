using LedgerFlow.Application.Expenses.Dtos;

namespace LedgerFlow.Application.Expenses.Interfaces;

public interface IExpenseService
{
    Task<List<ExpenseResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ExpenseResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ExpenseResponse> CreateAsync(
        CreateExpenseRequest request,
        CancellationToken cancellationToken = default);
}