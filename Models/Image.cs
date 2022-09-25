namespace ShopOfServices.Models
{
    public class Image
    {
        public const string EmptyImageName = "emptyImage.png";
        public const string UploadsFolderPath = @"images\Uploads";
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string GetPath()
        {
            return Path.Combine(@"\", UploadsFolderPath, Name);
        }
    }
}
