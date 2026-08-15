using LedgerFlow.Api.Authorization;
using LedgerFlow.Infrastructure.Identity;

using Microsoft.AspNetCore.Identity;

namespace LedgerFlow.Api.Identity;

public static class IdentitySeeder
{
    private sealed record SeedUser(
        string ConfigurationPrefix,
        string Role
    );


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


        /*
         * Ensure every LedgerFlow role exists.
         */
        foreach (var role in AppRoles.All)
        {
            await EnsureRoleAsync(
                roleManager,
                role
            );
        }


        /*
         * Development users.
         *
         * Credentials come from user-secrets,
         * never source control.
         */
        var seedUsers =
            new[]
            {
                new SeedUser(
                    "SeedAdmin",
                    AppRoles.Admin
                ),

                new SeedUser(
                    "SeedAccountant",
                    AppRoles.Accountant
                ),

                new SeedUser(
                    "SeedViewer",
                    AppRoles.Viewer
                )
            };


        foreach (var seedUser in seedUsers)
        {
            await EnsureUserAsync(
                userManager,
                configuration,
                seedUser
            );
        }
    }


    private static async Task EnsureRoleAsync(
        RoleManager<IdentityRole> roleManager,
        string roleName)
    {
        if (
            await roleManager
                .RoleExistsAsync(roleName)
        )
        {
            return;
        }


        var result =
            await roleManager.CreateAsync(
                new IdentityRole(roleName)
            );


        EnsureSucceeded(
            result,
            $"create role '{roleName}'"
        );
    }


    private static async Task EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        SeedUser seedUser)
    {
        var prefix =
            seedUser.ConfigurationPrefix;

        var email =
            configuration[
                $"{prefix}:Email"
            ];

        var password =
            configuration[
                $"{prefix}:Password"
            ];

        var displayName =
            configuration[
                $"{prefix}:DisplayName"
            ];


        /*
         * User is optional.
         *
         * If credentials aren't configured,
         * simply don't create the development user.
         */
        if (
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password)
        )
        {
            return;
        }


        email =
            email
                .Trim()
                .ToLowerInvariant();


        var user =
            await userManager
                .FindByEmailAsync(email);


        if (user is null)
        {
            user =
                new ApplicationUser
                {
                    UserName = email,
                    Email = email,

                    EmailConfirmed = true,

                    DisplayName =
                        string.IsNullOrWhiteSpace(
                            displayName
                        )
                            ? email
                            : displayName.Trim(),

                    CreatedAtUtc =
                        DateTime.UtcNow
                };


            var result =
                await userManager.CreateAsync(
                    user,
                    password
                );


            EnsureSucceeded(
                result,
                $"create user '{email}'"
            );
        }


        /*
         * Ensure correct role membership.
         */
        if (
            !await userManager.IsInRoleAsync(
                user,
                seedUser.Role
            )
        )
        {
            var roleResult =
                await userManager.AddToRoleAsync(
                    user,
                    seedUser.Role
                );


            EnsureSucceeded(
                roleResult,
                $"assign '{seedUser.Role}' to '{email}'"
            );
        }
    }


    private static void EnsureSucceeded(
        IdentityResult result,
        string operation)
    {
        if (result.Succeeded)
        {
            return;
        }


        var errors =
            string.Join(
                "; ",
                result.Errors.Select(
                    error =>
                        error.Description
                )
            );


        throw new InvalidOperationException(
            $"Unable to {operation}: {errors}"
        );
    }
}