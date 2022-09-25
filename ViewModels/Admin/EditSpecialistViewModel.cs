namespace ShopOfServices.ViewModels.Admin
{
    public class EditSpecialistViewModel
    {
        public Guid? Id { get; set; }

        public IFormFile? NewImageFile { get; set; }

        public string? OldImagePath { get; set; }

        public string FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string LastName { get; set; }

        public string Post { get; set; }

        public string Description { get; set; }

    }
}
