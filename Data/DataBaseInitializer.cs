using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ShopOfServices.Data
{
    public static class DataBaseInitializer
    {
        public static void Init(IServiceProvider scopeServiceProvider)
        {
            var userManager = scopeServiceProvider.GetService<UserManager<IdentityUser>>();


            var user = new IdentityUser
            {
                UserName = "bUNNY",
                Email = "cooes.ef@gmail.com",
            };

            var result = userManager.CreateAsync(user, "secretpass").GetAwaiter().GetResult();
            if (result.Succeeded)
            {
                userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, RoleNames.Administrator)).GetAwaiter().GetResult();
            }
            else
            {

                System.Diagnostics.Debug.WriteLine(string.Join("\n", result.Errors.Select(x => x.Description)));
            }
        }
    }
}
