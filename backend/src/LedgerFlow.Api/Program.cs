using LedgerFlow.Api.Authorization;
using LedgerFlow.Api.Identity;
using LedgerFlow.Application.Customers.Interfaces;
using LedgerFlow.Application.Customers.Services;
using LedgerFlow.Infrastructure.Identity;
using LedgerFlow.Infrastructure.Persistence;
using LedgerFlow.Infrastructure.Repositories;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using LedgerFlow.Application.Expenses.Interfaces;
using LedgerFlow.Application.Expenses.Services;
using LedgerFlow.Application.Reports.Interfaces;
using LedgerFlow.Application.Reports.Services;


var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found."
    );

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    );
});

// Add services to the container.


builder.Services.AddControllersWithViews(
    options =>
    {
        /*
         * Automatically validate XSRF tokens
         * for POST/PUT/PATCH/DELETE controller
         * actions.
         */
        options.Filters.Add(
            new AutoValidateAntiforgeryTokenAttribute());
    });

// builder.Services.AddAuthorization(
//     options =>
//     {
//         options.FallbackPolicy =
//             new AuthorizationPolicyBuilder()
//                 .RequireAuthenticatedUser()
//                 .Build();
//     });

builder.Services.AddAuthorization(options =>
{
    /*
     * Every endpoint requires authentication unless
     * explicitly marked with [AllowAnonymous].
     */
    options.AddPolicy(
    AppPolicies.CanViewExpenses,
    policy =>
        policy.RequireRole(
            AppRoles.Admin,
            AppRoles.Accountant,
            AppRoles.Viewer
        )
    );

    options.AddPolicy(
        AppPolicies.CanManageExpenses,
        policy =>
            policy.RequireRole(
                AppRoles.Admin,
                AppRoles.Accountant
            )
    );


    options.FallbackPolicy =
        new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();


    /*
     * Customers
     */
    options.AddPolicy(
        AppPolicies.CanViewCustomers,
        policy =>
            policy.RequireRole(
                AppRoles.Admin,
                AppRoles.Accountant,
                AppRoles.Viewer
            )
    );

    options.AddPolicy(
        AppPolicies.CanManageCustomers,
        policy =>
            policy.RequireRole(
                AppRoles.Admin,
                AppRoles.Accountant
            )
    );

    options.AddPolicy(
        AppPolicies.CanDeactivateCustomers,
        policy =>
            policy.RequireRole(
                AppRoles.Admin
            )
    );


    /*
     * Invoices
     */
    options.AddPolicy(
        AppPolicies.CanViewInvoices,
        policy =>
            policy.RequireRole(
                AppRoles.Admin,
                AppRoles.Accountant,
                AppRoles.Viewer
            )
    );

    options.AddPolicy(
        AppPolicies.CanManageInvoices,
        policy =>
            policy.RequireRole(
                AppRoles.Admin,
                AppRoles.Accountant
            )
    );


    /*
     * Payments
     */
    options.AddPolicy(
        AppPolicies.CanRecordPayments,
        policy =>
            policy.RequireRole(
                AppRoles.Admin,
                AppRoles.Accountant
            )
    );


    /*
     * Reports
     */
    options.AddPolicy(
        AppPolicies.CanViewReports,
        policy =>
            policy.RequireRole(
                AppRoles.Admin,
                AppRoles.Accountant,
                AppRoles.Viewer
            )
    );


    /*
     * Administration
     */
    options.AddPolicy(
        AppPolicies.CanManageUsers,
        policy =>
            policy.RequireRole(
                AppRoles.Admin
            )
    );
});


builder.Services.AddScoped<
    IExpenseRepository,
    ExpenseRepository>();

builder.Services.AddScoped<
    IExpenseService,
    ExpenseService>();

builder.Services.AddScoped<
    IReportRepository,
    ReportRepository>();

builder.Services.AddScoped<
    IReportService,
    ReportService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AngularDevelopment",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    );
});

builder.Services.AddScoped<
    ICustomerRepository,
    CustomerRepository>();

builder.Services.AddScoped<
    ICustomerService,
    CustomerService>();


builder.Services.AddAntiforgery(
    options =>
    {
        options.HeaderName =
            "X-XSRF-TOKEN";

        options.Cookie.Name =
            "LedgerFlow.Antiforgery";

        options.Cookie.HttpOnly =
            true;

        options.Cookie.SameSite =
            SameSiteMode.Lax;

        options.Cookie.SecurePolicy =
            builder.Environment.IsDevelopment()
                ? CookieSecurePolicy.SameAsRequest
                : CookieSecurePolicy.Always;
    });

builder.Services.AddAuthorization(
    options =>
    {
        options.FallbackPolicy =
            new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
    });

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 12;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;

        options.User.RequireUniqueEmail = true;

        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(15);

        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    await IdentitySeeder.SeedAsync(app);
}

app.UseRouting(); // app.UseHttpsRedirection();

app.UseCors("AngularDevelopment");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    // app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
