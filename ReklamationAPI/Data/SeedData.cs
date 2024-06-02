using Microsoft.AspNetCore.Identity;

namespace ReklamationAPI.data
{
    public class SeedData
    {
        /// <summary>
        /// Initializes the default roles and users for the application.
        /// </summary>
        /// <param name="serviceProvider">The service provider used for dependency injection.</param>
        /// <param name="userManager">The user manager used for managing users.</param>
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Define default role names.
            string[] roleNames = ["Admin", "User"];
            IdentityResult roleResult;

            // Create roles if they don't exist.
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create admin user if it doesn't exist.
            if (userManager.FindByNameAsync("admin").Result == null)
            {
                IdentityUser user = new()
                {
                    UserName = "admin"
                };

                // Create the admin user with a default password,
                IdentityResult result = userManager.CreateAsync(user, "Admin!123").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }

            // Create regular user if it doesn't exist.
            if (userManager.FindByNameAsync("user").Result == null)
            {
                IdentityUser user = new()
                {
                    UserName = "user"
                };

                // Create the regular user with a default password
                IdentityResult result = userManager.CreateAsync(user, "User!123").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "User").Wait();
                }
            }
        }
    }
}
