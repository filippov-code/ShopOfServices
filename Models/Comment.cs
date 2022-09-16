namespace ShopOfServices.Models
{
    public class Comment
    {
        public Guid Id { get; set; }

        public Service? Service { get; set; }

        public Comment? ResponseTo { get; set; }

        public string? SenderName { get; set; }

        public string? SenderEmail { get; set; }

        public string? Message { get; set; }

        public DateTime Date { get; set; }

        public bool IsPublished { get; set; }

        public Comment()
        { 
        
        }
    }
}
