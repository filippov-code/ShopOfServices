namespace ShopOfServices.ViewModels.Home
{
    public class NewCommentViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; }
    }
}
