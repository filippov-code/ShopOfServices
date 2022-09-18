namespace ShopOfServices.Models
{
    public class Specialist
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string LastName { get; set; }

        public Guid? ImageId { get; set; }

        public Image Image { get; set; }

        public string Description { get; set; }
    }
}
