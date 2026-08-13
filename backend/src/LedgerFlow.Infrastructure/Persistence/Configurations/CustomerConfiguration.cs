using LedgerFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LedgerFlow.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");

        builder.HasKey(customer => customer.Id);

        builder.Property(customer => customer.Id)
            .HasColumnName("id");

        builder.Property(customer => customer.CompanyName)
            .HasColumnName("company_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(customer => customer.ContactName)
            .HasColumnName("contact_name")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(customer => customer.Email)
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(customer => customer.Phone)
            .HasColumnName("phone")
            .HasMaxLength(50);

        builder.Property(customer => customer.AddressLine1)
            .HasColumnName("address_line_1")
            .HasMaxLength(255);

        builder.Property(customer => customer.AddressLine2)
            .HasColumnName("address_line_2")
            .HasMaxLength(255);

        builder.Property(customer => customer.City)
            .HasColumnName("city")
            .HasMaxLength(100);

        builder.Property(customer => customer.State)
            .HasColumnName("state")
            .HasMaxLength(100);

        builder.Property(customer => customer.PostalCode)
            .HasColumnName("postal_code")
            .HasMaxLength(20);

        builder.Property(customer => customer.Country)
            .HasColumnName("country")
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(customer => customer.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(customer => customer.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(customer => customer.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .IsRequired();

        builder.HasIndex(customer => customer.Email)
            .IsUnique();

        builder.HasIndex(customer => customer.CompanyName);
    }
}