using ShopOfServices.Models;

namespace ShopOfServices.ViewModels.Home
{
    public class ServiceViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ImagePath { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public ICollection<Specialist> Specialists { get; set; }
        public ICollection<Review> Comments { get; set; }
        public string[] AddCommentErrors { get; set; }
    }
}
