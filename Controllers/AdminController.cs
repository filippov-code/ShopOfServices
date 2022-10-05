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

        #region Pages
        public async Task<IActionResult> Main()
        {
            return await GetEditingPage(PageNames.Main, "Главная");
        }

        public async Task<IActionResult> About()
        {
            return await GetEditingPage(PageNames.About, "О нас");
        }
        public async Task<IActionResult> Price()
        {
            return await GetEditingPage(PageNames.Price, "Цены");
        }

        public async Task<IActionResult> Contacts()
        {
            return await GetEditingPage(PageNames.Contacts, "Контакты");
        }

        private async Task<IActionResult> GetEditingPage(string pageTypeName, string pageName)
        {
            Page page = await _siteDbContext.Pages.SingleOrDefaultAsync(x => x.Name == pageTypeName);
            EditPageViewModel pageViewModel = new EditPageViewModel { Id = page.Id, Html = page.Html };
            ViewBag.Title = $"Редактирование страницы \"{pageName}\"";
            return View("EditPage", pageViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SavePage(EditPageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Page page = await _siteDbContext.Pages.SingleOrDefaultAsync(x => x.Id == model.Id);
            if (page == null)
            {
                ModelState.AddModelError("", "Не найдена редактируемая страница. Попробуйте обновить страницу.");
                return View(model);
            }
            page.Html = model.Html;
            _siteDbContext.Pages.Update(page);
            await _siteDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        #endregion
        public IActionResult Services()
        {
            return View(_siteDbContext.Services.ToArray());
        }

        #region EditServices
        [HttpGet]
        public IActionResult EditService(Guid id)
        {
            var allSpecialistsCheckedBoxs = _siteDbContext
                .Specialists
                .Select(x => new SpecialistCheckBox 
                { 
                    Id = x.Id,
                    FIO = x.GetFIO(),
                    Post = x.Post
                })
                .ToArray();

            if (id != Guid.Empty)
            {
                Service service = _siteDbContext.Services.Include(x => x.Image).Include(x => x.Specialists).SingleOrDefault(s => s.Id == id);
                EditServiceViewModel model = new EditServiceViewModel
                {
                    Id = id,
                    Title = service.Title,
                    OldImagePath = service.Image.GetPath(),
                    ShortDescription = service.ShortDescription,
                    FullDescription = service.FullDescription,
                    Specialists = service.Specialists.ToArray(),
                    AllSpecialists = allSpecialistsCheckedBoxs
                };
                return View(model);
            }
            return View(new EditServiceViewModel { AllSpecialists = allSpecialistsCheckedBoxs});
        }

        [HttpPost]
        public async Task<IActionResult> EditService(EditServiceViewModel model)
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

                    newImage.Name = imageName;
                }
                else newImage.Name = Image.EmptyImageName;

                var newSpecialists = model.AllSpecialists
                    .Where(i => i.IsChecked)
                    .Select(x => _siteDbContext.Specialists
                    .Single(y => y.Id == x.Id))
                    .ToArray();

                Service service = new Service
                {
                    Title = model.Title,
                    Image = newImage,
                    ShortDescription = model.ShortDescription,
                    FullDescription = model.FullDescription,
                    Specialists = newSpecialists
                };

                _siteDbContext.Images.Add(newImage);
                _siteDbContext.Services.Add(service);
            }   
            else
            {
                Service service = await _siteDbContext.Services.Include(x => x.Image).Include(x=>x.Specialists).SingleAsync(x => x.Id == model.Id);
                if (model.NewImageFile != null)
                {
                    if (service.Image.Name != Image.EmptyImageName)
                    {
                        string imagesFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, Image.UploadsFolderPath);
                        string imagesForDeletePath = Path.Combine(imagesFolderPath, service.Image.Name);
                        System.IO.File.Delete(imagesForDeletePath);
                    }

                    string newImageName = GetUniqueFileName(model.NewImageFile.FileName);
                    string newImagePath = Path.Combine(_webHostEnvironment.WebRootPath, Image.UploadsFolderPath, newImageName);
                    using (var stream = new FileStream(newImagePath, FileMode.Create))
                    {
                        await model.NewImageFile.CopyToAsync(stream);
                    }

                    service.Image.Name = newImageName;
                    _siteDbContext.Images.Update(service.Image);
                }

                service.Title = model.Title;
                service.ShortDescription = model.ShortDescription;
                service.FullDescription = model.FullDescription;

                var checkedSpecialists = model.AllSpecialists.Where(x => x.IsChecked);
                var newSpecialists = checkedSpecialists.Select(x => _siteDbContext.Specialists.Single(u => u.Id == x.Id));

                service.Specialists.Clear();
                foreach (var specialist in newSpecialists)
                    service.Specialists.Add(specialist);

                _siteDbContext.Services.Update(service);
            }

            await _siteDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Services));
        }

        public IActionResult DeleteService(Guid id)
        {
            var service = _siteDbContext.Services
                .Include(x => x.Specialists)
                .Include(x => x.Image)
                .SingleOrDefault(x => x.Id == id);
            _siteDbContext.Services.Remove(service);
            _siteDbContext.SaveChanges();
            return RedirectToAction(nameof(Services));
        }
        #endregion

        public IActionResult Specialists()
        {
            return View(_siteDbContext.Specialists.ToArray());
        }
        
        #region EditSpecialists
        [HttpGet]
        public IActionResult EditSpecialist(Guid id)
        {
            if (id != Guid.Empty)
            {
                Specialist specialist = _siteDbContext.Specialists.Include(x => x.Image).SingleOrDefault(s => s.Id == id);
                EditSpecialistViewModel model = new EditSpecialistViewModel
                {
                    Id = id,
                    OldImagePath = specialist.Image.GetPath(),
                    FirstName = specialist.FirstName,
                    MiddleName = specialist.MiddleName,
                    LastName = specialist.LastName,
                    Post = specialist.Post,
                    Description = specialist.Description
                };
                return View(model);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditSpecialist(EditSpecialistViewModel model)
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


                    newImage.Name = imageName;
                }
                else newImage.Name = Image.EmptyImageName;

                Specialist specialist = new Specialist
                {
                    FirstName = model.FirstName,
                    MiddleName = model.MiddleName,
                    LastName = model.LastName,
                    Image = newImage,
                    Post = model.Post,
                    Description = model.Description
                };

                _siteDbContext.Images.Add(newImage);
                _siteDbContext.Specialists.Add(specialist);
            }
            else
            {
                Specialist specialist = await _siteDbContext.Specialists.Include(x => x.Image).SingleAsync(x => x.Id == model.Id);
                if (model.NewImageFile != null)
                {
                    if (specialist.Image.Name != Image.EmptyImageName)
                    {
                        string imagesFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, Image.UploadsFolderPath);
                        string imagesForDeletePath = Path.Combine(imagesFolderPath, specialist.Image.Name);
                        System.IO.File.Delete(imagesForDeletePath);
                    }

                    string newImageName = GetUniqueFileName(model.NewImageFile.FileName);
                    string newImagePath = Path.Combine(_webHostEnvironment.WebRootPath, Image.UploadsFolderPath, newImageName);
                    using (var stream = new FileStream(newImagePath, FileMode.Create))
                    {
                        await model.NewImageFile.CopyToAsync(stream);
                    }

                    specialist.Image.Name = newImageName;
                    _siteDbContext.Images.Update(specialist.Image);
                }

                specialist.FirstName = model.FirstName;
                specialist.MiddleName = model.MiddleName;
                specialist.LastName = model.LastName;
                specialist.Post = model.Post;
                specialist.Description = model.Description;

                _siteDbContext.Specialists.Update(specialist);
            }

            await _siteDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Specialists));
        }

        public IActionResult DeleteSpecialist(Guid id)
        {
            var specialist = _siteDbContext.Specialists
                .Include(x => x.Image)
                .Include(x => x.Services)
                .SingleOrDefault(x => x.Id == id);
            _siteDbContext.Specialists.Remove(specialist);
            _siteDbContext.SaveChanges();
            return RedirectToAction(nameof(Specialists));
        }
        #endregion

        private string GetUniqueFileName(string fileName)
        {
            return Guid.NewGuid().ToString().Substring(0, 4) +
                "_" +
                DateTime.Now.ToString("dd_mm_yyyy_HH_mm_ss") +
                Path.GetExtension(fileName);

        }

        public IActionResult Comments()
        {
            List<Comment> comments = _siteDbContext.Comments.Include(x => x.Service).ToList();
            return View(comments);
        }

        public async Task<IActionResult> PostComment(Guid id)
        {
            Comment comment = await _siteDbContext.Comments.SingleOrDefaultAsync(x => x.Id == id);
            comment.IsPublished = true;
            _siteDbContext.Comments.Update(comment);
            await _siteDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Comments), id);
        }

        public async Task<IActionResult> DeleteComment(Guid id)
        {
            Comment comment = await _siteDbContext.Comments.SingleOrDefaultAsync(x => x.Id == id);
            _siteDbContext.Comments.Remove(comment);
            await _siteDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Comments), id);
        }
    }
}
