using Microsoft.AspNetCore.Identity;
using ProgressTracker.Models;

namespace ProgressTracker.Data
{
    public class RoleSeeder
    {
        public static async Task SeedAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminLogin = "admin@nomail.com";
            string adminPassword = "0admin";
            string userLogin = "user@nomail.com";
            string userPassword = "0user";

            // Add roles
            if (await roleManager.FindByNameAsync(UserRoles.User) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            }
            if (await roleManager.FindByNameAsync(UserRoles.Admin) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }
            
            // Add users
            if (await userManager.FindByEmailAsync(adminLogin) == null)
            {
                IdentityUser admin = new IdentityUser
                {
                    Email = adminLogin,
                    UserName = "Admin",
                    EmailConfirmed = true
                };

                IdentityResult result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, UserRoles.Admin);
                }
            }
            if (await userManager.FindByEmailAsync(userLogin) == null)
            {
                IdentityUser user = new IdentityUser
                {
                    Email = userLogin,
                    UserName = "User",
                    EmailConfirmed = true
                };

                IdentityResult result = await userManager.CreateAsync(user, userPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, UserRoles.Admin);
                }
            }
        }
    }
}
