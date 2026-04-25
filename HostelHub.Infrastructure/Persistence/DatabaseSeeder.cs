using HostelHub.Domain.Constants;
using HostelHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HostelHub.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            // Seed Roles
            var roles = new[] { Roles.SuperAdmin, Roles.HostelAdmin, Roles.Guest };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed Super Admin
            var superAdminEmail = "superadmin@hostelhub.com";
            var superAdminUser = await userManager.FindByEmailAsync(superAdminEmail);
            if (superAdminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = superAdminEmail,
                    Email = superAdminEmail,
                    FirstName = "System",
                    LastName = "Administrator",
                    EmailConfirmed = true,
                    // No TenantId assigned because they span the system
                };

                var result = await userManager.CreateAsync(newAdmin, "SuperSecureP@ssw0rd!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, Roles.SuperAdmin);
                    logger.LogInformation("SuperAdmin successfully seeded.");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}
