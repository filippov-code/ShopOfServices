using System.ComponentModel.DataAnnotations;

namespace ShopOfServices.Models
{
    public class Comment
    {
        public Guid Id { get; set; }

        public Service Service { get; set; }

        public string SenderName { get; set; }

        public string SenderEmail { get; set; }

        public string Message { get; set; }

        public DateTime CreateAt { get; set; }

        public bool IsPublished { get; set; }
    }
}
