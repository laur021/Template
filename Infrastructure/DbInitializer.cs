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

            // Seed menu structure and permissions
            await SeedMenuStructureAsync(context, roleManager, logger);

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

    private static async Task SeedMenuStructureAsync(
        AppDbContext context,
        RoleManager<AppRole> roleManager,
        ILogger logger)
    {
        // Check if menu data already exists
        if (await context.MenuSections.AnyAsync())
        {
            logger.LogInformation("Menu structure already seeded");
            return;
        }

        logger.LogInformation("Seeding menu structure...");

        // Get roles
        var adminRole = await roleManager.FindByNameAsync("Admin");
        var userRole = await roleManager.FindByNameAsync("User");

        if (adminRole == null || userRole == null)
        {
            logger.LogWarning("Roles not found, skipping menu seeding");
            return;
        }

        // ============================================
        // SECTION 1: RECORDS
        // ============================================
        var recordsSection = new MenuSectionEntity
        {
            Id = Guid.NewGuid(),
            Name = "Records",
            Icon = "folder",
            DisplayOrder = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        context.MenuSections.Add(recordsSection);

        // Menu Items under Records
        var contactsMenu = new MenuItemEntity
        {
            Id = Guid.NewGuid(),
            SectionId = recordsSection.Id,
            Name = "Contacts",
            Icon = "contact",
            Route = "/records/contacts",
            DisplayOrder = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var employeesMenu = new MenuItemEntity
        {
            Id = Guid.NewGuid(),
            SectionId = recordsSection.Id,
            Name = "Employees",
            Icon = "people",
            Route = "/records/employees",
            DisplayOrder = 2,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var productsMenu = new MenuItemEntity
        {
            Id = Guid.NewGuid(),
            SectionId = recordsSection.Id,
            Name = "Products",
            Icon = "inventory",
            Route = "/records/products",
            DisplayOrder = 3,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.MenuItems.AddRange(contactsMenu, employeesMenu, productsMenu);

        // Sub-items under Contacts
        var phoneSubMenu = new MenuSubItemEntity
        {
            Id = Guid.NewGuid(),
            MenuItemId = contactsMenu.Id,
            Name = "Phone Directory",
            Icon = "phone",
            Route = "/records/contacts/phone",
            DisplayOrder = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var emailSubMenu = new MenuSubItemEntity
        {
            Id = Guid.NewGuid(),
            MenuItemId = contactsMenu.Id,
            Name = "Email Directory",
            Icon = "email",
            Route = "/records/contacts/email",
            DisplayOrder = 2,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.MenuSubItems.AddRange(phoneSubMenu, emailSubMenu);

        // ============================================
        // SECTION 2: REPORTS
        // ============================================
        var reportsSection = new MenuSectionEntity
        {
            Id = Guid.NewGuid(),
            Name = "Reports",
            Icon = "assessment",
            DisplayOrder = 2,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        context.MenuSections.Add(reportsSection);

        var salesReportMenu = new MenuItemEntity
        {
            Id = Guid.NewGuid(),
            SectionId = reportsSection.Id,
            Name = "Sales Reports",
            Icon = "trending_up",
            Route = "/reports/sales",
            DisplayOrder = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var inventoryReportMenu = new MenuItemEntity
        {
            Id = Guid.NewGuid(),
            SectionId = reportsSection.Id,
            Name = "Inventory Reports",
            Icon = "inventory_2",
            Route = "/reports/inventory",
            DisplayOrder = 2,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var analyticsMenu = new MenuItemEntity
        {
            Id = Guid.NewGuid(),
            SectionId = reportsSection.Id,
            Name = "Analytics",
            Icon = "analytics",
            Route = "/reports/analytics",
            DisplayOrder = 3,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.MenuItems.AddRange(salesReportMenu, inventoryReportMenu, analyticsMenu);

        // Sub-items under Analytics
        var dashboardSubMenu = new MenuSubItemEntity
        {
            Id = Guid.NewGuid(),
            MenuItemId = analyticsMenu.Id,
            Name = "Dashboard",
            Icon = "dashboard",
            Route = "/reports/analytics/dashboard",
            DisplayOrder = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var chartsSubMenu = new MenuSubItemEntity
        {
            Id = Guid.NewGuid(),
            MenuItemId = analyticsMenu.Id,
            Name = "Charts",
            Icon = "bar_chart",
            Route = "/reports/analytics/charts",
            DisplayOrder = 2,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.MenuSubItems.AddRange(dashboardSubMenu, chartsSubMenu);

        // ============================================
        // SECTION 3: SETTINGS (Admin only)
        // ============================================
        var settingsSection = new MenuSectionEntity
        {
            Id = Guid.NewGuid(),
            Name = "Settings",
            Icon = "settings",
            DisplayOrder = 3,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        context.MenuSections.Add(settingsSection);

        var userManagementMenu = new MenuItemEntity
        {
            Id = Guid.NewGuid(),
            SectionId = settingsSection.Id,
            Name = "User Management",
            Icon = "manage_accounts",
            Route = "/settings/users",
            DisplayOrder = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var roleManagementMenu = new MenuItemEntity
        {
            Id = Guid.NewGuid(),
            SectionId = settingsSection.Id,
            Name = "Role Management",
            Icon = "admin_panel_settings",
            Route = "/settings/roles",
            DisplayOrder = 2,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var menuManagementMenu = new MenuItemEntity
        {
            Id = Guid.NewGuid(),
            SectionId = settingsSection.Id,
            Name = "Menu Management",
            Icon = "menu_open",
            Route = "/settings/menus",
            DisplayOrder = 3,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var systemSettingsMenu = new MenuItemEntity
        {
            Id = Guid.NewGuid(),
            SectionId = settingsSection.Id,
            Name = "System Settings",
            Icon = "tune",
            Route = "/settings/system",
            DisplayOrder = 4,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.MenuItems.AddRange(userManagementMenu, roleManagementMenu, menuManagementMenu, systemSettingsMenu);

        // ============================================
        // PAGE ACTIONS
        // ============================================
        var allMenuItems = new[]
        {
            contactsMenu, employeesMenu, productsMenu,
            salesReportMenu, inventoryReportMenu, analyticsMenu,
            userManagementMenu, roleManagementMenu, menuManagementMenu, systemSettingsMenu
        };

        var pageActions = new List<PageActionEntity>();
        var actionTypes = new[]
        {
            ("create", "Create", "add"),
            ("edit", "Edit", "edit"),
            ("delete", "Delete", "delete"),
            ("view", "View", "visibility"),
            ("export", "Export", "download")
        };

        foreach (var menuItem in allMenuItems)
        {
            foreach (var (code, name, icon) in actionTypes)
            {
                pageActions.Add(new PageActionEntity
                {
                    Id = Guid.NewGuid(),
                    MenuItemId = menuItem.Id,
                    Code = code,
                    Name = name,
                    Description = $"{name} action for {menuItem.Name}",
                    DisplayOrder = Array.IndexOf(actionTypes, (code, name, icon)) + 1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        context.PageActions.AddRange(pageActions);
        await context.SaveChangesAsync();

        // ============================================
        // ROLE PERMISSIONS
        // ============================================

        // Admin has access to ALL menus
        var adminMenuAccess = new List<RoleMenuAccessEntity>();
        var adminActionAccess = new List<RoleActionAccessEntity>();

        // Admin - All Sections
        foreach (var section in new[] { recordsSection, reportsSection, settingsSection })
        {
            adminMenuAccess.Add(new RoleMenuAccessEntity
            {
                Id = Guid.NewGuid(),
                RoleId = adminRole.Id,
                SectionId = section.Id,
                HasAccess = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Admin - All Menu Items
        foreach (var item in allMenuItems)
        {
            adminMenuAccess.Add(new RoleMenuAccessEntity
            {
                Id = Guid.NewGuid(),
                RoleId = adminRole.Id,
                MenuItemId = item.Id,
                HasAccess = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Admin - All Sub Items
        foreach (var subItem in new[] { phoneSubMenu, emailSubMenu, dashboardSubMenu, chartsSubMenu })
        {
            adminMenuAccess.Add(new RoleMenuAccessEntity
            {
                Id = Guid.NewGuid(),
                RoleId = adminRole.Id,
                SubItemId = subItem.Id,
                HasAccess = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Admin - All Actions
        foreach (var action in pageActions)
        {
            adminActionAccess.Add(new RoleActionAccessEntity
            {
                Id = Guid.NewGuid(),
                RoleId = adminRole.Id,
                ActionId = action.Id,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        context.RoleMenuAccess.AddRange(adminMenuAccess);
        context.RoleActionAccess.AddRange(adminActionAccess);

        // ============================================
        // User Role - Limited Access (Records & Reports only, no Settings)
        // ============================================
        var userMenuAccess = new List<RoleMenuAccessEntity>();
        var userActionAccess = new List<RoleActionAccessEntity>();

        // User - Records and Reports sections only
        foreach (var section in new[] { recordsSection, reportsSection })
        {
            userMenuAccess.Add(new RoleMenuAccessEntity
            {
                Id = Guid.NewGuid(),
                RoleId = userRole.Id,
                SectionId = section.Id,
                HasAccess = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        // User - Records and Reports menu items only
        var userMenuItems = new[] { contactsMenu, employeesMenu, productsMenu, salesReportMenu, inventoryReportMenu, analyticsMenu };
        foreach (var item in userMenuItems)
        {
            userMenuAccess.Add(new RoleMenuAccessEntity
            {
                Id = Guid.NewGuid(),
                RoleId = userRole.Id,
                MenuItemId = item.Id,
                HasAccess = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        // User - All Sub Items under accessible menus
        foreach (var subItem in new[] { phoneSubMenu, emailSubMenu, dashboardSubMenu, chartsSubMenu })
        {
            userMenuAccess.Add(new RoleMenuAccessEntity
            {
                Id = Guid.NewGuid(),
                RoleId = userRole.Id,
                SubItemId = subItem.Id,
                HasAccess = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        // User - Limited actions (view and export only, no create/edit/delete)
        var userAllowedActions = new[] { "view", "export" };
        var userPageActions = pageActions
            .Where(a => userMenuItems.Any(m => m.Id == a.MenuItemId) && userAllowedActions.Contains(a.Code));

        foreach (var action in userPageActions)
        {
            userActionAccess.Add(new RoleActionAccessEntity
            {
                Id = Guid.NewGuid(),
                RoleId = userRole.Id,
                ActionId = action.Id,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        context.RoleMenuAccess.AddRange(userMenuAccess);
        context.RoleActionAccess.AddRange(userActionAccess);

        await context.SaveChangesAsync();

        logger.LogInformation("Menu structure seeded successfully with {SectionCount} sections, {MenuCount} menu items, {SubMenuCount} sub-items, and {ActionCount} actions",
            3, allMenuItems.Length, 4, pageActions.Count);
    }
}
