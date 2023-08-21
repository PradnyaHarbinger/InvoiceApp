using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using InvoiceApp.Authorization;

namespace InvoiceApp.Data
{
    public class SeedData
    {
        public static async Task Initialize(
            IServiceProvider serviceProvider,
            string password = "Test@12345")
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // manager
                var managerUid = await EnsureUser(serviceProvider, "manager@demo.com", password);
                await EnsureRole(serviceProvider, managerUid, Constants.InvoiceManagerRole);
            }
        }
        private static async Task<string> EnsureUser(
            IServiceProvider serviceProvider,
            string userName, string initPw)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            //calling seeding everytime application starts
            var user = await userManager.FindByNameAsync(userName);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = userName,
                    Email = userName,
                    EmailConfirmed = true,

                };

                var result = await userManager.CreateAsync(user, initPw);
            }

            if(user == null)
            {
                throw new Exception("User did not get created. Password policy problem?");
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(
            IServiceProvider serviceProvider,
            string uid, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            IdentityResult ir;

            if(await roleManager.RoleExistsAsync(role) == false) // if thi specific role does not exist
            {
                ir = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();


            var user = await userManager.FindByIdAsync(uid);

            if(user == null)
            {
                throw new Exception("User not existing");
            }

            ir = await userManager.AddToRoleAsync(user, role);

            return ir;
        }
    }
}
