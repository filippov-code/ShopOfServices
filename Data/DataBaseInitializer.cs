using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using ShopOfServices.Models;

namespace ShopOfServices.Data
{
    public static class DataBaseInitializer
    {
        public static void Init(IServiceProvider scopeServiceProvider)
        {
            //add admin
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
            //add empty image
            if (dbContext.Images.Where(x => x.Name == Image.EmptyImageName).Count() == 0)
                dbContext.Images.Add(new Image { Id = Guid.Empty, Name = "emptyImage"});

            //add pages
            if (dbContext.Pages.Count() == 0)
            {
                dbContext.Pages.Add(new Page { Name = PageNames.Main, Html = "<h1>Главная страница</h1>" });
                dbContext.Pages.Add(new Page { Name = PageNames.Price, Html = "<h1>Страница с ценами</h1>" });
                dbContext.Pages.Add(new Page { Name = PageNames.About, Html = "<h1>О нас</h1>" });
                dbContext.Pages.Add(new Page { Name = PageNames.Contacts, Html = "<h1>Контакты</h1>" });
            }


            dbContext.SaveChanges();
        }
    }
}
