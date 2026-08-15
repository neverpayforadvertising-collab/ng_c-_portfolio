using LedgerFlow.Api.Authorization;
using LedgerFlow.Api.Identity;

using LedgerFlow.Application.Customers.Interfaces;
using LedgerFlow.Application.Customers.Services;
using LedgerFlow.Application.Expenses.Interfaces;
using LedgerFlow.Application.Expenses.Services;
using LedgerFlow.Application.Reports.Interfaces;
using LedgerFlow.Application.Reports.Services;

using LedgerFlow.Infrastructure.Identity;
using LedgerFlow.Infrastructure.Persistence;
using LedgerFlow.Infrastructure.Repositories;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


/*
 * ============================================================
 * Database
 * ============================================================
 */

var connectionString =
    builder.Configuration.GetConnectionString(
        "DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found.");

builder.Services.AddDbContext<AppDbContext>(
    options =>
    {
        options.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(
                connectionString));
    });


/*
 * ============================================================
 * ASP.NET Core Identity
 * ============================================================
 */

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(
        options =>
        {
            /*
             * Password policy
             */
            options.Password.RequiredLength = 12;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;

            /*
             * User policy
             */
            options.User.RequireUniqueEmail = true;

            /*
             * Lockout policy
             */
            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.MaxFailedAccessAttempts = 5;

            options.Lockout.DefaultLockoutTimeSpan =
                TimeSpan.FromMinutes(15);

            /*
             * Development:
             * seeded users do not need email confirmation.
             */
            options.SignIn.RequireConfirmedEmail = false;
        })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


/*
 * ============================================================
 * Authentication cookie
 * ============================================================
 */

builder.Services.ConfigureApplicationCookie(
    options =>
    {
        options.Cookie.Name =
            "LedgerFlow.Auth";

        options.Cookie.HttpOnly =
            true;

        options.Cookie.Path =
            "/";

        options.Cookie.SameSite =
            SameSiteMode.Lax;

        options.Cookie.SecurePolicy =
            builder.Environment.IsDevelopment()
                ? CookieSecurePolicy.SameAsRequest
                : CookieSecurePolicy.Always;

        options.ExpireTimeSpan =
            TimeSpan.FromHours(8);

        options.SlidingExpiration =
            true;

        /*
         * APIs should return HTTP status codes
         * instead of redirecting to HTML login pages.
         */
        options.Events.OnRedirectToLogin =
            context =>
            {
                context.Response.StatusCode =
                    StatusCodes.Status401Unauthorized;

                return Task.CompletedTask;
            };

        options.Events.OnRedirectToAccessDenied =
            context =>
            {
                context.Response.StatusCode =
                    StatusCodes.Status403Forbidden;

                return Task.CompletedTask;
            };
    });


/*
 * ============================================================
 * Antiforgery / XSRF
 * ============================================================
 */

builder.Services.AddAntiforgery(
    options =>
    {
        /*
         * Angular sends this header automatically
         * for same-origin mutating requests.
         */
        options.HeaderName =
            "X-XSRF-TOKEN";

        /*
         * Private server-side antiforgery cookie.
         *
         * AuthController /api/auth/csrf can expose
         * the separate XSRF-TOKEN cookie Angular reads.
         */
        options.Cookie.Name =
            "LedgerFlow.Antiforgery";

        options.Cookie.HttpOnly =
            true;

        options.Cookie.Path =
            "/";

        options.Cookie.SameSite =
            SameSiteMode.Lax;

        options.Cookie.SecurePolicy =
            builder.Environment.IsDevelopment()
                ? CookieSecurePolicy.SameAsRequest
                : CookieSecurePolicy.Always;
    });


/*
 * ============================================================
 * Controllers
 * ============================================================
 */

builder.Services.AddControllersWithViews(
    options =>
    {
        /*
         * Automatically validates antiforgery tokens
         * for unsafe HTTP operations such as:
         *
         * POST
         * PUT
         * PATCH
         * DELETE
         */
        options.Filters.Add(
            new AutoValidateAntiforgeryTokenAttribute());
    });


/*
 * ============================================================
 * Authorization / RBAC
 * ============================================================
 */

builder.Services.AddAuthorization(
    options =>
    {
        /*
         * Secure everything by default.
         *
         * Public endpoints must explicitly use:
         *
         * [AllowAnonymous]
         */
        options.FallbackPolicy =
            new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();


        /*
         * ----------------------------------------------------
         * Customers
         * ----------------------------------------------------
         */

        options.AddPolicy(
            AppPolicies.CanViewCustomers,
            policy =>
                policy.RequireRole(
                    AppRoles.Admin,
                    AppRoles.Accountant,
                    AppRoles.Viewer));

        options.AddPolicy(
            AppPolicies.CanManageCustomers,
            policy =>
                policy.RequireRole(
                    AppRoles.Admin,
                    AppRoles.Accountant));

        options.AddPolicy(
            AppPolicies.CanDeactivateCustomers,
            policy =>
                policy.RequireRole(
                    AppRoles.Admin));


        /*
         * ----------------------------------------------------
         * Invoices
         * ----------------------------------------------------
         */

        options.AddPolicy(
            AppPolicies.CanViewInvoices,
            policy =>
                policy.RequireRole(
                    AppRoles.Admin,
                    AppRoles.Accountant,
                    AppRoles.Viewer));

        options.AddPolicy(
            AppPolicies.CanManageInvoices,
            policy =>
                policy.RequireRole(
                    AppRoles.Admin,
                    AppRoles.Accountant));


        /*
         * ----------------------------------------------------
         * Payments
         * ----------------------------------------------------
         */

        options.AddPolicy(
            AppPolicies.CanRecordPayments,
            policy =>
                policy.RequireRole(
                    AppRoles.Admin,
                    AppRoles.Accountant));


        /*
         * ----------------------------------------------------
         * Expenses
         * ----------------------------------------------------
         */

        options.AddPolicy(
            AppPolicies.CanViewExpenses,
            policy =>
                policy.RequireRole(
                    AppRoles.Admin,
                    AppRoles.Accountant,
                    AppRoles.Viewer));

        options.AddPolicy(
            AppPolicies.CanManageExpenses,
            policy =>
                policy.RequireRole(
                    AppRoles.Admin,
                    AppRoles.Accountant));


        /*
         * ----------------------------------------------------
         * Reports
         * ----------------------------------------------------
         */

        options.AddPolicy(
            AppPolicies.CanViewReports,
            policy =>
                policy.RequireRole(
                    AppRoles.Admin,
                    AppRoles.Accountant,
                    AppRoles.Viewer));


        /*
         * ----------------------------------------------------
         * Administration
         * ----------------------------------------------------
         */

        options.AddPolicy(
            AppPolicies.CanManageUsers,
            policy =>
                policy.RequireRole(
                    AppRoles.Admin));
    });


/*
 * ============================================================
 * Application services / repositories
 * ============================================================
 */

/*
 * Customers
 */
builder.Services.AddScoped<
    ICustomerRepository,
    CustomerRepository>();

builder.Services.AddScoped<
    ICustomerService,
    CustomerService>();


/*
 * Expenses
 */
builder.Services.AddScoped<
    IExpenseRepository,
    ExpenseRepository>();

builder.Services.AddScoped<
    IExpenseService,
    ExpenseService>();


/*
 * Reports
 */
builder.Services.AddScoped<
    IReportRepository,
    ReportRepository>();

builder.Services.AddScoped<
    IReportService,
    ReportService>();


/*
 * ============================================================
 * CORS
 * ============================================================
 *
 * Mainly useful for non-proxied local development.
 *
 * Docker/Nginx uses same-origin /api requests, so CORS
 * is not normally involved there.
 */

builder.Services.AddCors(
    options =>
    {
        options.AddPolicy(
            "AngularDevelopment",
            policy =>
            {
                policy
                    .WithOrigins(
                        "http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
    });


/*
 * ============================================================
 * Swagger
 * ============================================================
 */

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


/*
 * ============================================================
 * Build application
 * ============================================================
 */

var app = builder.Build();


/*
 * ============================================================
 * Database migrations
 * ============================================================
 *
 * CRITICAL:
 *
 * Migrations MUST run before IdentitySeeder.
 *
 * Otherwise IdentitySeeder queries AspNetRoles / AspNetUsers
 * before those tables exist in a fresh Docker database.
 */

if (
    app.Configuration.GetValue<bool>(
        "Database:ApplyMigrations"))
{
    using var scope =
        app.Services.CreateScope();

    var dbContext =
        scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

    await dbContext.Database.MigrateAsync();
}


/*
 * ============================================================
 * Development initialization
 * ============================================================
 */

if (app.Environment.IsDevelopment())
{
    /*
     * Database schema already exists at this point,
     * so RoleManager/UserManager can safely query
     * Identity tables.
     */
    await IdentitySeeder.SeedAsync(app);

    app.UseSwagger();
    app.UseSwaggerUI();
}


/*
 * ============================================================
 * HTTP middleware pipeline
 * ============================================================
 */

app.UseRouting();

/*
 * Keep before Authentication / Authorization.
 */
app.UseCors(
    "AngularDevelopment");


/*
 * Authentication MUST run before Authorization.
 */
app.UseAuthentication();

app.UseAuthorization();


/*
 * Controller endpoints
 */
app.MapControllers();


/*
 * ============================================================
 * Start application
 * ============================================================
 */

app.Run();