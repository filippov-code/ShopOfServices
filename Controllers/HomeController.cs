using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopOfServices.Models;
using System.Reflection;

namespace ShopOfServices.Controllers
{
    public class HomeController : Controller
    {
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
                var services = new List<Service>
                {
                    new Service { Title = "Услуга 1", ShortDescription = "Краткое описание услуги 1" },
                    new Service { Title = "Услуга 2", ShortDescription = "Краткое описание услуги 2" },
                    new Service { Title = "Услуга 3", ShortDescription = "Краткое описание услуги 3" }
                };

                return View(services);
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
