using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopOfServices.Data;
using ShopOfServices.ViewModels.Admin;

namespace ShopOfServices.Controllers
{
    [Authorize(Policy = "Administrator")]
    public class AdminController : Controller
    {
        private SiteDbContext _siteDbContext;


        public AdminController(SiteDbContext siteDbContext)
        {
            _siteDbContext = siteDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
