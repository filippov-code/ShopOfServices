namespace ShopOfServices.Models
{
    public class ResponseToComment
    {
        public Guid Id { get; set; }

        public Guid ResponseToId { get; set; }

        public Comment ResponseTo { get; set; }

        public string Response { get; set; }
    }
}
