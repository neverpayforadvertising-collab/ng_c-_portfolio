// using LedgerFlow.Application.Customers.Interfaces;
using global::LedgerFlow.Application.Customers.Interfaces;

using LedgerFlow.Domain.Entities;
using LedgerFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LedgerFlow.Infrastructure.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _dbContext;

    public CustomerRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Customer>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers
            .AsNoTracking()
            .OrderByDescending(customer => customer.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<Customer?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(
                customer => customer.Id == id,
                cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Customers
            .AnyAsync(
                customer => customer.Email == email,
                cancellationToken);
    }

    public async Task AddAsync(
        Customer customer,
        CancellationToken cancellationToken = default)
    {
        _dbContext.Customers.Add(customer);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}