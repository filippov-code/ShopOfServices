using Microsoft.AspNetCore.Mvc.Rendering;
using ShopOfServices.Models;

namespace ShopOfServices.ViewModels.Home
{
    public class AddReviewViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public Category[]? Categories { get; set; }
        public Guid SelectedServiceId { get; set; }
    }
}
