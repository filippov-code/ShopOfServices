using ShopOfServices.Models;
using System.ComponentModel.DataAnnotations;

namespace ShopOfServices.ViewModels.Admin
{
    public class ServiceViewModel
    {
        public Guid? Id { get; set; }
        public IFormFile? NewImageFile { get; set; }
        public string? OldImagePath { get; set; }
        public string Title { get; set; }
        //public Guid ImageId { get; set; }
        //public Image Image { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
    }
}
