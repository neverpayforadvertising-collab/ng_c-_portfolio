using LedgerFlow.Infrastructure.Identity;

using Microsoft.AspNetCore.Identity;

namespace LedgerFlow.Api.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        WebApplication app)
    {
        using var scope =
            app.Services.CreateScope();

        var userManager =
            scope.ServiceProvider
                .GetRequiredService<
                    UserManager<ApplicationUser>>();

        var roleManager =
            scope.ServiceProvider
                .GetRequiredService<
                    RoleManager<IdentityRole>>();

        var configuration =
            scope.ServiceProvider
                .GetRequiredService<
                    IConfiguration>();


        const string adminRole =
            "Admin";


        if (!await roleManager
            .RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(
                new IdentityRole(adminRole));
        }


        var email =
            configuration[
                "SeedAdmin:Email"];

        var password =
            configuration[
                "SeedAdmin:Password"];

        var displayName =
            configuration[
                "SeedAdmin:DisplayName"];


        if (
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            return;
        }


        var existing =
            await userManager
                .FindByEmailAsync(email);


        if (existing is not null)
        {
            return;
        }


        var user =
            new ApplicationUser
            {
                UserName = email,

                Email = email,

                EmailConfirmed = true,

                DisplayName =
                    displayName ??
                    "Administrator",

                CreatedAtUtc =
                    DateTime.UtcNow
            };


        var result =
            await userManager
                .CreateAsync(
                    user,
                    password);


        if (!result.Succeeded)
        {
            var errors =
                string.Join(
                    ", ",
                    result.Errors.Select(
                        error =>
                            error.Description));

            throw new InvalidOperationException(
                $"Unable to seed administrator: {errors}");
        }


        await userManager
            .AddToRoleAsync(
                user,
                adminRole);
    }
}