using Flippit.IdentityProvider.DAL;
using Flippit.IdentityProvider.DAL.Entities;
using Flippit.Api.DAL.EF;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Flippit.Api.App.Services;

public class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, string databaseProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRoleEntity>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUserEntity>>();
        var identityDbContext = scope.ServiceProvider.GetRequiredService<IdentityProviderDbContext>();

        // For SQL Server, apply migrations. For InMemory, ensure database is created.
        if (identityDbContext.Database.IsSqlServer())
        {
            await identityDbContext.Database.MigrateAsync();
        }
        else
        {
            await identityDbContext.Database.EnsureCreatedAsync();
        }

        // Initialize FlippitDbContext if using SqlServer provider
        if (databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            var flippitDbContext = scope.ServiceProvider.GetService<FlippitDbContext>();
            if (flippitDbContext != null)
            {
                if (flippitDbContext.Database.IsSqlServer())
                {
                    await flippitDbContext.Database.MigrateAsync();
                }
                else
                {
                    await flippitDbContext.Database.EnsureCreatedAsync();
                }
            }
        }
        
        await SeedRolesAsync(roleManager);
        await SeedUsersAsync(userManager);
    }

    private static async Task SeedRolesAsync(RoleManager<AppRoleEntity> roleManager)
    {
        var roles = new[] { "Admin", "User" };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new AppRoleEntity
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant()
                };
                await roleManager.CreateAsync(role);
            }
        }
    }

    private static async Task SeedUsersAsync(UserManager<AppUserEntity> userManager)
    {
        // Seed admin user
        var adminEmail = "admin@flippit.local";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new AppUserEntity
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true,
                Active = true,
                Subject = Guid.NewGuid().ToString()
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Seed regular user
        var userEmail = "user@flippit.local";
        if (await userManager.FindByEmailAsync(userEmail) == null)
        {
            var regularUser = new AppUserEntity
            {
                UserName = "user",
                Email = userEmail,
                EmailConfirmed = true,
                Active = true,
                Subject = Guid.NewGuid().ToString()
            };

            var result = await userManager.CreateAsync(regularUser, "User123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(regularUser, "User");
            }
        }
    }
}
