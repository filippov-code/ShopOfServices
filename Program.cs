using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopOfServices.Data;
using System;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SiteDbContext>(x => x.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ))

.AddIdentity<IdentityUser, IdentityRole>(config =>
{
    config.Password.RequireDigit = true;
    config.Password.RequireLowercase = false;
    config.Password.RequireUppercase = false;
    config.Password.RequiredLength = 9;
    config.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<SiteDbContext>();

builder.Services.ConfigureApplicationCookie(config =>
{
    config.LoginPath = "/Admin/Login";
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(RoleNames.Administrator, builder =>
    {
        builder.RequireClaim(ClaimTypes.Role, RoleNames.Administrator);
    });
});

builder.Services.AddControllersWithViews();

builder.Services.AddRouting(config => config.LowercaseUrls = true);

//========
var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseStaticFiles();

app.UseDefaultFiles();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

using (var scope = app.Services.CreateScope())
{
    DataBaseInitializer.Init(scope.ServiceProvider);
}

app.Run();
