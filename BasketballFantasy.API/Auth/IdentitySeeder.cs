using BasketballFantasy.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace BasketballFantasy.API.Auth
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            foreach (var role in new[] { ApplicationRoles.Admin, ApplicationRoles.User })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
                }
            }

            const string adminUserName = "admin";
            const string adminEmail = "admin@basketballfantasy.local";

            if (await userManager.FindByNameAsync(adminUserName) != null)
            {
                return;
            }

            var adminUser = new ApplicationUser
            {
                UserName = adminUserName,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User"
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, ApplicationRoles.Admin);
            }
        }
    }
}
