using LedgerFlow.Application.Customers.Dtos;
using LedgerFlow.Application.Customers.Interfaces;
using LedgerFlow.Domain.Entities;

namespace LedgerFlow.Application.Customers.Services;

public sealed class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(
        ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<List<CustomerResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var customers =
            await _customerRepository.GetAllAsync(
                cancellationToken);

        return customers
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<CustomerResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var customer =
            await _customerRepository.GetByIdAsync(
                id,
                cancellationToken);

        return customer is null
            ? null
            : MapToResponse(customer);
    }

    public async Task<CustomerResponse> CreateAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail =
            request.Email.Trim().ToLowerInvariant();

        var exists =
            await _customerRepository.EmailExistsAsync(
                normalizedEmail,
                cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException(
                "A customer with this email already exists.");
        }

        var now = DateTime.UtcNow;

        var customer = new Customer
        {
            Id = Guid.NewGuid(),

            CompanyName =
                request.CompanyName.Trim(),

            ContactName =
                request.ContactName.Trim(),

            Email = normalizedEmail,

            Phone = Clean(request.Phone),

            AddressLine1 =
                Clean(request.AddressLine1),

            AddressLine2 =
                Clean(request.AddressLine2),

            City = Clean(request.City),

            State = Clean(request.State),

            PostalCode =
                Clean(request.PostalCode),

            Country =
                request.Country
                    .Trim()
                    .ToUpperInvariant(),

            IsActive = true,

            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        await _customerRepository.AddAsync(
            customer,
            cancellationToken);

        return MapToResponse(customer);
    }

    private static string? Clean(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static CustomerResponse MapToResponse(
        Customer customer)
    {
        return new CustomerResponse
        {
            Id = customer.Id,
            CompanyName = customer.CompanyName,
            ContactName = customer.ContactName,
            Email = customer.Email,
            Phone = customer.Phone,
            City = customer.City,
            State = customer.State,
            Country = customer.Country,
            IsActive = customer.IsActive,
            CreatedAtUtc = customer.CreatedAtUtc
        };
    }
}