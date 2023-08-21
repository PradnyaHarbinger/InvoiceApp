using Microsoft.AspNetCore.Identity;

namespace InvoiceApp.Data
{
    public class SeedData
    {
        private static async Task<string> EnsureUser(
            IServiceProvider serviceProvider,
            string initPw, string userName)
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

        public static async Task<IdentityResult> EnsureRole(
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
