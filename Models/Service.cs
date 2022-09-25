namespace ShopOfServices.Models
{
    public class Service
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Image Image { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public ICollection<Specialist> Specialists { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
