using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Runtime.CompilerServices;

namespace ShopOfServices.Models.Configurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            //builder
            //    .HasOne(x => x.Image);

            //builder
            //    .HasMany(x => x.Specialists)
            //    .WithMany(x => x.Services)
            //    .UsingEntity(

            builder.HasKey(x => x.Id);

        }
    }
}
