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
    }
}
