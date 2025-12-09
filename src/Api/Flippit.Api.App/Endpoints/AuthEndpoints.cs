using Flippit.Api.App.Models;
using Flippit.Api.App.Services;
using Flippit.IdentityProvider.DAL.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Flippit.Api.App.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder routeGroupBuilder)
    {
        var authEndPoints = routeGroupBuilder.MapGroup("auth")
            .WithTags("authentication");

        authEndPoints.MapPost("/login", async Task<Results<Ok<LoginResponse>, UnauthorizedHttpResult, BadRequest<string>>> (
            [FromBody] LoginRequest request,
            [FromServices] UserManager<AppUserEntity> userManager,
            [FromServices] JwtTokenService jwtTokenService) =>
        {
            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
            {
                return TypedResults.BadRequest("Username and password are required");
            }

            var user = await userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return TypedResults.Unauthorized();
            }

            var isValidPassword = await userManager.CheckPasswordAsync(user, request.Password);
            if (!isValidPassword)
            {
                return TypedResults.Unauthorized();
            }

            var token = await jwtTokenService.GenerateTokenAsync(user);
            var roles = await userManager.GetRolesAsync(user);

            var response = new LoginResponse
            {
                Token = token,
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                Roles = roles
            };

            return TypedResults.Ok(response);
        }).AllowAnonymous();

        authEndPoints.MapPost("/register", async Task<Results<Ok<RegisterResponse>, BadRequest<string>>> (
            [FromBody] RegisterRequest request,
            [FromServices] UserManager<AppUserEntity> userManager,
            [FromServices] RoleManager<AppRoleEntity> roleManager) =>
        {
            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
            {
                return TypedResults.BadRequest("Username and password are required");
            }

            var existingUser = await userManager.FindByNameAsync(request.UserName);
            if (existingUser != null)
            {
                return TypedResults.BadRequest("User already exists");
            }

            var user = new AppUserEntity
            {
                UserName = request.UserName,
                Email = request.Email,
                Subject = Guid.NewGuid().ToString()
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return TypedResults.BadRequest($"Failed to create user: {errors}");
            }

            var defaultRole = "User";
            if (!await roleManager.RoleExistsAsync(defaultRole))
            {
                await roleManager.CreateAsync(new AppRoleEntity { Name = defaultRole });
            }

            await userManager.AddToRoleAsync(user, defaultRole);

            var response = new RegisterResponse
            {
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty
            };

            return TypedResults.Ok(response);
        }).AllowAnonymous();

        return routeGroupBuilder;
    }
}
