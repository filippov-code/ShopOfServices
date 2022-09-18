using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ShopOfServices.Models.Configurations
{
    public class ResponseToCommentConfiguration : IEntityTypeConfiguration<ResponseToComment>
    {
        public void Configure(EntityTypeBuilder<ResponseToComment> builder)
        {
            builder.HasKey(x => x.Id);

            builder
                .HasOne(x => x.ResponseTo)
                .WithOne()
                .IsRequired();

            builder.Property(x => x.Response).IsRequired();
        }
    }
}
