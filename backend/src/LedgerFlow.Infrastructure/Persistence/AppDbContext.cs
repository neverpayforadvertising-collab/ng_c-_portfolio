using LedgerFlow.Domain.Entities;
using LedgerFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LedgerFlow.Infrastructure.Persistence;

public sealed class AppDbContext
    : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers =>
        Set<Customer>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        /*
         * Configure ASP.NET Core Identity tables first.
         */
        base.OnModelCreating(modelBuilder);

        /*
         * Automatically load EF Core configurations
         * such as CustomerConfiguration.
         */
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly
        );
    }
}

using LedgerFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LedgerFlow.Infrastructure.Persistence.Configurations;

public sealed class ExpenseConfiguration
    : IEntityTypeConfiguration<Expense>
{
    public void Configure(
        EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("expenses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Vendor)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.Category)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.ExpenseDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.Reference)
            .HasMaxLength(100);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.ExpenseDate);

        builder.HasIndex(x => x.Category);
    }
}

public DbSet<Customer> Customers =>
    Set<Customer>();

public DbSet<Expense> Expenses =>
    Set<Expense>();
