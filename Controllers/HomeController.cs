﻿using Microsoft.AspNetCore.Html;
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
            return View(_siteDbContext.Services.Include(x => x.Image).ToArray());
        }

        public IActionResult Service(Guid id)
        {
            Service service = _siteDbContext
                .Services
                .Include(x => x.Image)
                .Include(x => x.Specialists)
                .Include(x => x.Comments)
                .SingleOrDefault(x => x.Id == id);

            return View(service);
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

        public async Task<IActionResult> AddComment(NewCommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            Service service = await _siteDbContext.Services.SingleOrDefaultAsync(x => x.Id == model.ServiceId);
            if (service == null)
            {
                ModelState.AddModelError("", "Что-то пошло не так");
                return View(model);
            }

            Comment comment = new Comment
            {
                CreateAt = DateTime.Now,
                SenderEmail = model.Email,
                SenderName = model.Name,
                Service = service,
                IsPublished = false,
                Message = model.Message
            };

            await _siteDbContext.Comments.AddAsync(comment);
            await _siteDbContext.SaveChangesAsync();

            return View();
        }

        public IActionResult Contacts()
        {
            return View();
        }
    }
}
