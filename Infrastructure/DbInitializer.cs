using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

/// <summary>
/// Database initializer for seeding initial data.
/// </summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

            // Apply migrations automatically
            logger.LogInformation("Applying database migrations...");
            await context.Database.MigrateAsync();

            // Seed roles
            await SeedRolesAsync(roleManager, logger);

            // Seed admin user with profile
            await SeedAdminUserAsync(userManager, context, logger);

            logger.LogInformation("Database initialization completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }

    private static async Task SeedRolesAsync(
        RoleManager<AppRole> roleManager,
        ILogger logger)
    {
        var roles = new[]
        {
            new AppRole("Admin") { Description = "Full system access" },
            new AppRole("User") { Description = "Standard user access" }
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
            {
                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    logger.LogInformation("Created role: {RoleName}", role.Name);
                }
                else
                {
                    logger.LogWarning(
                        "Failed to create role {RoleName}: {Errors}",
                        role.Name,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    private static async Task SeedAdminUserAsync(
        UserManager<AppUser> userManager,
        AppDbContext context,
        ILogger logger)
    {
        const string adminEmail = "admin@example.com";
        const string adminPassword = "Admin123!"; // Change in production!

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true, // Pre-confirmed for seeded user
                DisplayName = "Administrator",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRolesAsync(adminUser, new[] { "Admin", "User" });

                // Create user profile with extended details
                var adminProfile = new UserProfileEntity
                {
                    Id = adminUser.Id,
                    FirstName = "System",
                    LastName = "Administrator",
                    DisplayName = "Administrator",
                    Bio = "System administrator account",
                    PhoneNumber = "+1234567890",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Gender = "Not Specified",
                    Address = "123 Admin Street",
                    City = "Admin City",
                    State = "Admin State",
                    Country = "United States",
                    PostalCode = "12345",
                    CreatedAt = DateTime.UtcNow
                };

                context.UserProfiles.Add(adminProfile);
                await context.SaveChangesAsync();

                logger.LogInformation(
                    "Created admin user: {Email} with password: {Password}",
                    adminEmail, adminPassword);
            }
            else
            {
                logger.LogWarning(
                    "Failed to create admin user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            // Ensure profile exists for existing admin user
            var profileExists = await context.UserProfiles.AnyAsync(p => p.Id == adminUser.Id);
            if (!profileExists)
            {
                var adminProfile = new UserProfileEntity
                {
                    Id = adminUser.Id,
                    FirstName = "System",
                    LastName = "Administrator",
                    DisplayName = "Administrator",
                    Bio = "System administrator account",
                    CreatedAt = DateTime.UtcNow
                };

                context.UserProfiles.Add(adminProfile);
                await context.SaveChangesAsync();
                logger.LogInformation("Created profile for existing admin user: {Email}", adminEmail);
            }

            logger.LogInformation("Admin user already exists: {Email}", adminEmail);
        }
    }
}
