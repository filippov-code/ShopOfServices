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


        public async Task<IActionResult> About()
        {
            return await GetEditingPage(PageNames.About, "О нас");
        }
        public async Task<IActionResult> Price()
        {
            return null;
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
        public IActionResult CategoriesAndServices()
        {
            return View(_siteDbContext.Categories.Include(x => x.Services).ToList());
        }

        #region EditServices
        [HttpGet]
        public IActionResult EditCategory(Guid id)
        {
            if (id != Guid.Empty)
            {
                Category category = _siteDbContext.Categories.Include(x => x.Image).SingleOrDefault(s => s.Id == id);
                EditCategoryViewModel model = new EditCategoryViewModel
                {
                    Id = id,
                    Name = category.Name,
                    OldImagePath = category.Image.GetPath(),
                    Description = category.Description,
                    Services = category.Services.ToArray()
                };
                return View(model);
            }
            return View(new EditCategoryViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(EditCategoryViewModel model)
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

                Category category = new Category
                {
                    Name = model.Name,
                    Description = model.Description,
                    Services = model.Services,
                    Image = newImage
                };

                _siteDbContext.Images.Add(newImage);
                _siteDbContext.Categories.Add(category);
            }   
            else
            {
                Category category = await _siteDbContext.Categories.Include(x => x.Image).SingleAsync(x => x.Id == model.Id);
                if (model.NewImageFile != null)
                {
                    if (category.Image.Name != Image.EmptyImageName)
                    {
                        string imagesFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, Image.UploadsFolderPath);
                        string imagesForDeletePath = Path.Combine(imagesFolderPath, category.Image.Name);
                        System.IO.File.Delete(imagesForDeletePath);
                    }

                    string newImageName = GetUniqueFileName(model.NewImageFile.FileName);
                    string newImagePath = Path.Combine(_webHostEnvironment.WebRootPath, Image.UploadsFolderPath, newImageName);
                    using (var stream = new FileStream(newImagePath, FileMode.Create))
                    {
                        await model.NewImageFile.CopyToAsync(stream);
                    }

                    category.Image.Name = newImageName;
                    _siteDbContext.Images.Update(category.Image);
                }

                category.Name = model.Name;
                category.Description = model.Description;

                category.Services.Clear();
                foreach (var service in model.Services)
                {
                    category.Services.Add(service);
                }

                _siteDbContext.Categories.Update(category);
            }

            await _siteDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(CategoriesAndServices));
        }

        public IActionResult DeleteCategory(Guid id)
        {
            Category category = _siteDbContext.Categories
                .Include(x => x.Image)
                .SingleOrDefault(x => x.Id == id);
            _siteDbContext.Categories.Remove(category);
            _siteDbContext.SaveChanges();
            return RedirectToAction(nameof(CategoriesAndServices));
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
