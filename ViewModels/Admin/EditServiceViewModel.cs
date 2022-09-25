using ShopOfServices.Models;
using System.ComponentModel.DataAnnotations;

namespace ShopOfServices.ViewModels.Admin
{
    public class EditServiceViewModel
    {
        public Guid? Id { get; set; }
        public IFormFile? NewImageFile { get; set; }
        public string? OldImagePath { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public Specialist[]? Specialists { get; set; }
        public SpecialistCheckBox[] AllSpecialists { get; set; }
    }
}
