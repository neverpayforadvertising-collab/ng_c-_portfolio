using LedgerFlow.Api.Contracts.Auth;
using LedgerFlow.Infrastructure.Identity;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LedgerFlow.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController
    : ControllerBase
{
    private readonly UserManager<ApplicationUser>
        _userManager;

    private readonly SignInManager<ApplicationUser>
        _signInManager;

    private readonly IWebHostEnvironment
        _environment;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IWebHostEnvironment environment)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _environment = environment;
    }


    /*
     * Angular calls this before a mutating request.
     *
     * ASP.NET stores its private antiforgery cookie
     * and we expose the request token in the
     * XSRF-TOKEN cookie expected by Angular.
     */
    [AllowAnonymous]
    [HttpGet("csrf")]
    public IActionResult GetCsrfToken(
        [FromServices] IAntiforgery antiforgery)
    {
        var tokens =
            antiforgery.GetAndStoreTokens(
                HttpContext);

        Response.Cookies.Append(
            "XSRF-TOKEN",
            tokens.RequestToken!,
            new CookieOptions
            {
                HttpOnly = false,

                Secure =
                    !_environment.IsDevelopment(),

                SameSite =
                    SameSiteMode.Lax,

                Path = "/"
            });

        return NoContent();
    }


    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<
        ActionResult<CurrentUserResponse>
    > Login(
        LoginRequest request)
    {
        var email =
            request.Email
                .Trim()
                .ToLowerInvariant();

        var user =
            await _userManager
                .FindByEmailAsync(email);

        /*
         * Keep the response generic to avoid
         * unnecessary account enumeration.
         */
        if (user is null)
        {
            return Unauthorized(
                new ProblemDetails
                {
                    Status =
                        StatusCodes.Status401Unauthorized,

                    Title =
                        "Login failed",

                    Detail =
                        "Invalid email or password."
                });
        }


        var result =
            await _signInManager
                .PasswordSignInAsync(
                    user,
                    request.Password,
                    request.RememberMe,
                    lockoutOnFailure: true);


        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                return StatusCode(
                    StatusCodes.Status423Locked,
                    new ProblemDetails
                    {
                        Status =
                            StatusCodes.Status423Locked,

                        Title =
                            "Account temporarily locked",

                        Detail =
                            "Too many failed login attempts. Please try again later."
                    });
            }

            return Unauthorized(
                new ProblemDetails
                {
                    Status =
                        StatusCodes.Status401Unauthorized,

                    Title =
                        "Login failed",

                    Detail =
                        "Invalid email or password."
                });
        }


        return Ok(
            await BuildResponseAsync(user));
    }


    [HttpGet("me")]
    public async Task<
        ActionResult<CurrentUserResponse>
    > Me()
    {
        var user =
            await _userManager
                .GetUserAsync(User);

        if (user is null)
        {
            return Unauthorized();
        }

        return Ok(
            await BuildResponseAsync(user));
    }


    [HttpPost("logout")]
    public async Task<IActionResult>
        Logout()
    {
        await _signInManager.SignOutAsync();

        return NoContent();
    }


    private async Task<CurrentUserResponse>
        BuildResponseAsync(
            ApplicationUser user)
    {
        var roles =
            await _userManager
                .GetRolesAsync(user);

        return new CurrentUserResponse
        {
            Id = user.Id,

            Email =
                user.Email ?? string.Empty,

            DisplayName =
                user.DisplayName,

            Roles =
                roles.ToArray()
        };
    }
}