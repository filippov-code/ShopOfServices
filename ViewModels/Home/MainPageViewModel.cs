using ShopOfServices.Models;

namespace ShopOfServices.ViewModels.Home
{
    public class MainPageViewModel
    {
        public Category[] Categories { get; set; }
        public Specialist[] Specialists { get; set; }
        public Review[] Reviews { get; set; } 
    }
}
