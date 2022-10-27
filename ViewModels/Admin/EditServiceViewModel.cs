using ShopOfServices.Models;
using System.ComponentModel.DataAnnotations;

namespace ShopOfServices.ViewModels.Admin
{
    public class EditServiceViewModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
    }
}
