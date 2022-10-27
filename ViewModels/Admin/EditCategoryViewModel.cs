using ShopOfServices.Models;

namespace ShopOfServices.ViewModels.Admin
{
    public class EditCategoryViewModel
    {
        public Guid? Id { get; set; }
        public IFormFile? NewImageFile { get; set; }
        public string? OldImagePath { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] ServiceNames { get; set; }
    }
}
