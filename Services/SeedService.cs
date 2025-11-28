using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Configuration;

namespace RoleBasedAuthentication.Services
{
    public class SeedService
    {
        public static async Task SeedDatabase(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Data.AppDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager=scope.ServiceProvider.GetRequiredService<UserManager<Models.Users>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedService>>();

            try
            {
                //Ensure Database is ready
                logger.LogInformation("Ensuring database is created...");
                await context.Database.EnsureCreatedAsync();

                //Seed Roles
                logger.LogInformation("Seeding roles...");
                await AddRoleAsync(roleManager, "Admin");
                await AddRoleAsync(roleManager, "Users");

                //Seed Admin User
                logger.LogInformation("Seeding admin user...");
                var adminEmail = "admin123@gmail.com";
                if(await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var adminUser = new Models.Users
                    {
                        UserName = adminEmail,
                        NormalizedUserName = adminEmail.ToUpper(),
                        NormalizedEmail = adminEmail.ToUpper(),
                        Email = adminEmail,
                        FullName = "Admin User",
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };
                    var result = await userManager.CreateAsync(adminUser, "Admin@123");
                    if(result.Succeeded)
                    {
                        logger.LogInformation("Assigning admin role to admin user");
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                    else
                    {
                        logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }

        private static async Task AddRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if(!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if(!result.Succeeded)
                {
                    throw new Exception($"Failed to create role {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
                else
                {
                    Console.WriteLine($"Role {roleName} created successfully.");
                }

            }
        }
    }
}
