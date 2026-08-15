using LedgerFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using LedgerFlow.Application.Customers.Interfaces;
using LedgerFlow.Application.Customers.Services;
using LedgerFlow.Infrastructure.Repositories;
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


builder.Services.AddControllers(
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

builder.Services.AddAuthorization(
    options =>
    {
        options.FallbackPolicy =
            new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
    });


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
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
