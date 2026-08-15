using LedgerFlow.Domain.Entities;

namespace LedgerFlow.Application.Expenses.Interfaces;

public interface IExpenseRepository
{
    Task<List<Expense>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Expense?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Expense expense,
        CancellationToken cancellationToken = default);
}
