﻿using Microsoft.AspNetCore.Identity;
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

            var result = userManager.CreateAsync(user, "secretpass22").GetAwaiter().GetResult();
            if (result.Succeeded)
            {
                userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, RoleNames.Administrator)).GetAwaiter().GetResult();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("========================================================");
                System.Diagnostics.Debug.WriteLine(string.Join("\n", result.Errors.Select(x => x.Description)));
                System.Diagnostics.Debug.WriteLine("========================================================");
            }


            var dbContext = scopeServiceProvider.GetService<SiteDbContext>();
            dbContext.Images.Add(new Models.Image { Id = Guid.Empty, Path = "emptyImage"});
            dbContext.SaveChanges();
        }
    }
}
