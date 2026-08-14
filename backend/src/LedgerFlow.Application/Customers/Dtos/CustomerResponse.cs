using System.ComponentModel.DataAnnotations;

namespace LedgerFlow.Application.Customers.Dtos;

public sealed class CreateCustomerRequest
{
    [Required]
    [MaxLength(200)]
    public string CompanyName { get; init; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string ContactName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; init; } = string.Empty;

    [MaxLength(50)]
    public string? Phone { get; init; }

    [MaxLength(255)]
    public string? AddressLine1 { get; init; }

    [MaxLength(255)]
    public string? AddressLine2 { get; init; }

    [MaxLength(100)]
    public string? City { get; init; }

    [MaxLength(100)]
    public string? State { get; init; }

    [MaxLength(20)]
    public string? PostalCode { get; init; }

    [Required]
    [StringLength(2, MinimumLength = 2)]
    public string Country { get; init; } = "US";
}