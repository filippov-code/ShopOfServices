using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopOfServices.Data;
using ShopOfServices.Models;
using ShopOfServices.ViewModels.Home;
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

        public async Task<IActionResult> Index()
        {
            Page page = await _siteDbContext.Pages.Where(x => x.Name == PageNames.Main).SingleOrDefaultAsync();
            //if (page != null)
                return View(model: page.Html);
            //return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Services()
        {
            return View(_siteDbContext.Services.Include(x => x.Image).ToArray());
        }

        public async Task<IActionResult> Service(Guid id)
        {
            Service service = await _siteDbContext
                .Services
                .Include(x => x.Image)
                .Include(x => x.Specialists)
                .Include(x => x.Comments)
                .SingleOrDefaultAsync(x => x.Id == id);

            ServiceViewModel serviceViewModel = new ServiceViewModel
            {
                Id = service.Id,
                Title = service.Title,
                ImagePath = service.Image.GetPath(),
                ShortDescription = service.ShortDescription,
                FullDescription = service.FullDescription,
                Specialists = service.Specialists,
                Comments = service.Comments
            };

            return View(serviceViewModel);
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

        public IActionResult Comments()
        {
            return View();
        }

        public async Task<IActionResult> AddComment(Guid id)
        {
            Service service = await _siteDbContext.Services.SingleOrDefaultAsync(x => x.Id == id);
            NewCommentViewModel newCommnetViewModel = new NewCommentViewModel
            {
                ServiceName = service.Title,
                ServiceId = service.Id
            };
            return View(newCommnetViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(NewCommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Service service = await _siteDbContext.Services.SingleOrDefaultAsync(x => x.Id == model.ServiceId);
            if (service == null)
            {
                ModelState.AddModelError("", "Такая услуга не найдена");
                return View(model);
            }

            Comment comment = new Comment()
            {
                CreateAt = DateTime.Now,
                SenderEmail = model.Email,
                SenderName = model.Name,
                IsPublished = false,
                Message = model.Message,
                Service = service
            };

            await _siteDbContext.Comments.AddAsync(comment);
            await _siteDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Service), new { id = model.ServiceId });
        }

        public IActionResult Contacts()
        {
            return View();
        }
    }
}
