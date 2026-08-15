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
