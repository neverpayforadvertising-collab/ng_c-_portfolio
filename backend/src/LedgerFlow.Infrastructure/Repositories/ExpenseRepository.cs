using LedgerFlow.Application.Expenses.Interfaces;
using LedgerFlow.Domain.Entities;
using LedgerFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LedgerFlow.Infrastructure.Repositories;

public sealed class ExpenseRepository
    : IExpenseRepository
{
    private readonly AppDbContext _dbContext;

    public ExpenseRepository(
        AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Expense>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Expenses
            .AsNoTracking()
            .Where(expense =>
                !expense.IsArchived)
            .OrderByDescending(
                expense =>
                    expense.ExpenseDate)
            .ThenByDescending(
                expense =>
                    expense.CreatedAtUtc)
            .ToListAsync(
                cancellationToken);
    }

    public async Task<Expense?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Expenses
            .AsNoTracking()
            .FirstOrDefaultAsync(
                expense =>
                    expense.Id == id &&
                    !expense.IsArchived,
                cancellationToken);
    }

    public async Task AddAsync(
        Expense expense,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Expenses.Add(expense);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}