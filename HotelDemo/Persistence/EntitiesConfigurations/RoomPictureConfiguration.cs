using HotelDemo.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelDemo.Persistence.EntitiesConfigurations
{
    public class RoomPictureConfiguration : IEntityTypeConfiguration<RoomPicture>
    {
        public void Configure(EntityTypeBuilder<RoomPicture> builder)
        {
            builder.ToTable("RoomPictures");
            builder.HasKey(rp => rp.Id);
            builder.Property(rp => rp.PictureUrl)
                .IsRequired();
            builder.Property(x => x.RoomId)
                   .IsRequired();

            builder.ConfigureBaseModel();
        }
    }
}
