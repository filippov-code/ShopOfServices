using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            //    var mainPageModel = new MainPageViewModel
            //    {
            //        Categories = _siteDbContext.Categories.Include(x => x.Image).ToArray(),
            //        Specialists = _siteDbContext.Specialists.Include(x => x.Image).ToArray(),
            //        Reviews = _siteDbContext.Reviews.Where(x => x.IsPublished).Include(x => x.Service).Take(9).ToArray()
            //    };

            return View(
                (
                _siteDbContext.Categories.Include(x => x.Image).ToArray(),
                _siteDbContext.Specialists.Include(x => x.Image).ToArray(),
                _siteDbContext.Reviews.Where(x => x.IsPublished).Include(x => x.Service).Take(9).ToArray()
                )
            ); ;
        }

        public async Task<IActionResult> About()
        {
            Page page = await _siteDbContext.Pages.Where(x => x.Name == PageNames.About).SingleOrDefaultAsync();

            return View(model: page.Html);
        }

        public IActionResult Services()
        {
            var categories = _siteDbContext.Categories.Include(x => x.Image).ToArray();
            return View(categories);
        }

        public async Task<IActionResult> Service(Guid id)
        {
            var category = await _siteDbContext.Categories
                .Include(x => x.Services)
                .Include(x => x.Image)
                .SingleAsync(x => x.Id == id);
            return View(category);
        }

        public IActionResult Specialists()
        {
            var specialists = _siteDbContext.Specialists.Include(x => x.Image).ToArray();
            return View(specialists);
        }


        public async Task<IActionResult> Prices()
        {
            var categories = await _siteDbContext.Categories.Include(x => x.Services).ToArrayAsync();
            return View(categories);
        }

        public async Task<IActionResult> Reviews()
        {
            var reviews = _siteDbContext.Reviews
                .Where(x => x.IsPublished)
                .Include(x => x.Service).Take(18)
                .ToArray();

            return View(reviews);
        }

        public async Task<IActionResult> AddReview()
        {
            var categories = _siteDbContext.Categories.Include(x => x.Services).ToArray();
            var reviewModel = new AddReviewViewModel
            {
                Categories = categories
            };

            return View(reviewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(AddReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.SelectedServiceId == Guid.Empty)
            {
                ModelState.AddModelError("", "Выберите услугу");
                return View(model);
            }
            var selectedService = await _siteDbContext.Services.SingleOrDefaultAsync(x => x.Id == model.SelectedServiceId);
            if (selectedService == null)
            {
                ModelState.AddModelError("", "Такая услуга не найдена");
                return View(model);
            }
            var review = new Review
            {
                SenderEmail = model.Email,
                SenderName = model.Name,
                CreateAt = DateTime.Now,
                IsPublished = false,
                Message = model.Message,
                Service = selectedService
            };
            await _siteDbContext.Reviews.AddAsync(review);
            await _siteDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Contacts()
        {
            Page page = await _siteDbContext.Pages.Where(x => x.Name == PageNames.Contacts).SingleOrDefaultAsync();

            return View(model: page.Html);
        }
    }
}
