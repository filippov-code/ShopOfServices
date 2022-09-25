using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopOfServices.Data;
using ShopOfServices.Models;
using System.Reflection;

namespace ShopOfServices.Controllers
{
    public class HomeController : Controller
    {
        SiteDbContext _siteDbContext;

        public HomeController(SiteDbContext siteDbContext)
        {
            _siteDbContext = siteDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Services()
        {
            return View(_siteDbContext.Services.Include(x => x.Image).ToList());
        }

        public IActionResult Service(Guid id)
        {
            return View(id);
        }

        public IActionResult Specialists()
        {
            var specialists = new List<Specialist>
            {
                new Specialist { FirstName = "FIO 1" },
                new Specialist { FirstName = "FIO 2" },
                new Specialist { FirstName = "FIO 3" }
            };
            return View(specialists);
        }

        public IActionResult Specialist(Guid id)
        {
            return View(id);
        }

        public IActionResult Prices()
        {
            return View();
        }

        public IActionResult Reviews()
        {
            return View();
        }

        public IActionResult Contacts()
        {
            return View();
        }
    }
}
