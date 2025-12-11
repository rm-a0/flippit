using Flippit.IdentityProvider.DAL;
using Flippit.IdentityProvider.DAL.Entities;
using Flippit.Api.BL.Facades;
using Flippit.Api.DAL.Common.Repositories;
using Flippit.Api.DAL.Common.Entities;
using Flippit.Common.Models.User;
using Flippit.Common.Models.Collection;
using Flippit.Common.Models.Card;
using Flippit.Common.Enums;
using Microsoft.AspNetCore.Identity;

namespace Flippit.Api.App.Services;

public class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRoleEntity>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUserEntity>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityProviderDbContext>();

        await dbContext.Database.EnsureCreatedAsync();
        await SeedRolesAsync(roleManager);
        var seededUsers = await SeedUsersAsync(userManager);
        
        // Seed API users, collections and cards
        await SeedApiDataAsync(scope.ServiceProvider, seededUsers);
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

    private static async Task<Dictionary<string, Guid>> SeedUsersAsync(UserManager<AppUserEntity> userManager)
    {
        var seededUsers = new Dictionary<string, Guid>();
        
        // Seed admin user
        var adminEmail = "admin@flippit.local";
        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin == null)
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
                seededUsers["admin"] = adminUser.Id;
            }
        }
        else
        {
            seededUsers["admin"] = existingAdmin.Id;
        }

        // Seed regular user
        var userEmail = "user@flippit.local";
        var existingUser = await userManager.FindByEmailAsync(userEmail);
        if (existingUser == null)
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
                seededUsers["user"] = regularUser.Id;
            }
        }
        else
        {
            seededUsers["user"] = existingUser.Id;
        }
        
        return seededUsers;
    }
    
    private static async Task SeedApiDataAsync(IServiceProvider serviceProvider, Dictionary<string, Guid> seededUsers)
    {
        if (!seededUsers.ContainsKey("admin") || !seededUsers.ContainsKey("user"))
        {
            return;
        }

        var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
        var collectionRepository = serviceProvider.GetRequiredService<ICollectionRepository>();
        var cardRepository = serviceProvider.GetRequiredService<ICardRepository>();

        var adminId = seededUsers["admin"];
        var userId = seededUsers["user"];

        // Seed API users
        var existingUsers = userRepository.GetAll();
        if (!existingUsers.Any(u => u.Id == adminId))
        {
            userRepository.Insert(new UserEntity
            {
                Id = adminId,
                Name = "Admin User",
                PhotoUrl = "https://i.pravatar.cc/150?u=admin",
                Role = Role.Admin
            });
        }

        if (!existingUsers.Any(u => u.Id == userId))
        {
            userRepository.Insert(new UserEntity
            {
                Id = userId,
                Name = "Regular User",
                PhotoUrl = "https://i.pravatar.cc/150?u=user",
                Role = Role.User
            });
        }

        // Seed collections
        var existingCollections = collectionRepository.GetAll();
        if (!existingCollections.Any())
        {
            var collection1Id = Guid.NewGuid();
            var collection2Id = Guid.NewGuid();

            collectionRepository.Insert(new CollectionEntity
            {
                Id = collection1Id,
                Name = "Geography Basics",
                CreatorId = userId,
                StartTime = DateTime.UtcNow.AddDays(-1),
                EndTime = DateTime.UtcNow.AddDays(30)
            });

            collectionRepository.Insert(new CollectionEntity
            {
                Id = collection2Id,
                Name = "Math Fundamentals",
                CreatorId = userId,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddDays(60)
            });

            var collection3Id = Guid.NewGuid();
            collectionRepository.Insert(new CollectionEntity
            {
                Id = collection3Id,
                Name = "Admin's Collection",
                CreatorId = adminId,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddDays(90)
            });

            // Seed cards for collection 1 (Geography)
            cardRepository.Insert(new CardEntity
            {
                Id = Guid.NewGuid(),
                QuestionType = QAType.Text,
                AnswerType = QAType.Text,
                Question = "What is the capital of France?",
                Answer = "Paris",
                Description = "Basic geography question",
                CreatorId = userId,
                CollectionId = collection1Id
            });

            cardRepository.Insert(new CardEntity
            {
                Id = Guid.NewGuid(),
                QuestionType = QAType.Text,
                AnswerType = QAType.Text,
                Question = "What is the largest ocean?",
                Answer = "Pacific Ocean",
                Description = "Geography question",
                CreatorId = userId,
                CollectionId = collection1Id
            });

            // Seed cards for collection 2 (Math)
            cardRepository.Insert(new CardEntity
            {
                Id = Guid.NewGuid(),
                QuestionType = QAType.Text,
                AnswerType = QAType.Text,
                Question = "What is 2 + 2?",
                Answer = "4",
                Description = "Basic arithmetic",
                CreatorId = userId,
                CollectionId = collection2Id
            });

            cardRepository.Insert(new CardEntity
            {
                Id = Guid.NewGuid(),
                QuestionType = QAType.Text,
                AnswerType = QAType.Text,
                Question = "What is 5 * 6?",
                Answer = "30",
                Description = "Multiplication",
                CreatorId = userId,
                CollectionId = collection2Id
            });

            // Seed cards for collection 3 (Admin's)
            cardRepository.Insert(new CardEntity
            {
                Id = Guid.NewGuid(),
                QuestionType = QAType.Text,
                AnswerType = QAType.Text,
                Question = "What is the speed of light?",
                Answer = "299,792,458 m/s",
                Description = "Physics question",
                CreatorId = adminId,
                CollectionId = collection3Id
            });
        }
        
        await Task.CompletedTask; // Keep async signature for consistency
    }
}
