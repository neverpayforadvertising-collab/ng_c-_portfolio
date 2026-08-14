using LedgerFlow.Application.Customers.Dtos;

namespace LedgerFlow.Application.Customers.Interfaces;

public interface ICustomerService
{
    Task<List<CustomerResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<CustomerResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<CustomerResponse> CreateAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken = default);
}