using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ShopOfServices.Models.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder
                .HasOne(x => x.Service)
                .WithMany(x => x.Comments)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Service).IsRequired();
            builder.Property(x => x.ResponseTo);
            builder.Property(x => x.SenderName).IsRequired();
            builder.Property(x => x.SenderEmail).IsRequired();
            builder.Property(x => x.Message).IsRequired();
            builder.Property(x => x.Date);
            builder.Property(x => x.IsPublished).IsRequired();
        }
    }
}
