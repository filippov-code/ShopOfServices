namespace ShopOfServices.Models
{
    public class Service
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public ICollection<Review> Comments { get; set; }
        public Category Category { get; set; }
    }
}
