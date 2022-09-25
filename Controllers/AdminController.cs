using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ShopOfServices.Data;
using ShopOfServices.ViewModels.Admin;
using ShopOfServices.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace ShopOfServices.Controllers
{
    [Authorize(Policy = "Administrator")]
    public class AdminController : Controller
    {
        private SiteDbContext _siteDbContext;
        private SignInManager<IdentityUser> _signInManager;
        private UserManager<IdentityUser> _userManager;
        private IWebHostEnvironment _webHostEnvironment;

        public AdminController(
            SiteDbContext siteDbContext, 
            SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _siteDbContext = siteDbContext;
            _signInManager = signInManager;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            ModelState.AddModelError("", "Неверные данные");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Services()
        {
            return View(_siteDbContext.Services.ToArray());
        }

        [HttpGet]
        public IActionResult AddOrUpdateService(Guid id)
        {
            if (id != Guid.Empty)
            {
                Service service = _siteDbContext.Services.Include(x => x.Image).SingleOrDefault(s => s.Id == id);
                ServiceViewModel model = new ServiceViewModel
                {
                    Id = id,
                    Title = service.Title,
                    OldImagePath = "/images/uploads/" + service.Image.Path,
                    ShortDescription = service.ShortDescription,
                    FullDescription = service.FullDescription
                };
                return View(model);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateService(ServiceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Id == null)
            {
                Image newImage = new Image();
                if (model.NewImageFile != null)
                {
                    string imageName = GetUniqueFileName(model.NewImageFile.FileName);
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, Image.UploadsFolderPath, imageName);
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await model.NewImageFile.CopyToAsync(stream);
                    }
                    

                    newImage.Path = imageName;
                }
                else newImage.Path = Image.EmptyImageName;

                Service service = new Service
                {
                    Title = model.Title,
                    Image = newImage,
                    ShortDescription = model.ShortDescription,
                    FullDescription = model.FullDescription
                };

                _siteDbContext.Images.Add(newImage);
                _siteDbContext.Services.Add(service);
            }   
            else
            {
                Service service = await _siteDbContext.Services.Include(x => x.Image).SingleAsync(x => x.Id == model.Id);
                if (model.NewImageFile != null)
                {
                    if (service.Image.Path != Image.EmptyImageName)
                    {
                        string imagesFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, Image.UploadsFolderPath);
                        string imagesForDeletePath = Path.Combine(imagesFolderPath, service.Image.Path);
                        System.IO.File.Delete(imagesForDeletePath);
                    }

                    string newImageName = GetUniqueFileName(model.NewImageFile.FileName);
                    string newImagePath = Path.Combine(_webHostEnvironment.WebRootPath, Image.UploadsFolderPath, newImageName);
                    using (var stream = new FileStream(newImagePath, FileMode.Create))
                    {
                        await model.NewImageFile.CopyToAsync(stream);
                    }

                    service.Image.Path = newImageName;
                    _siteDbContext.Images.Update(service.Image);
                }

                service.Title = model.Title;
                service.ShortDescription = model.ShortDescription;
                service.FullDescription = model.FullDescription;

                _siteDbContext.Update(service);
            }

            _siteDbContext.SaveChanges();
            return RedirectToAction("Services");
        }

        private string GetUniqueFileName(string fileName)
        {
            return Guid.NewGuid().ToString().Substring(0, 4) +
                "_" +
                DateTime.Now.ToString("dd_mm_yyyy_HH_mm_ss") +
                Path.GetExtension(fileName);

        }
    }
}
