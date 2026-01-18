using HotelDemo.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelDemo.Persistence.EntitiesConfigurations
{
    public class RoomOfferConfiguration
        : IEntityTypeConfiguration<RoomOffer>
    {
        public void Configure(EntityTypeBuilder<RoomOffer> builder)
        {
            builder.ToTable("RoomOffers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RoomId)
                   .IsRequired();

            builder.Property(x => x.OfferId)
                   .IsRequired();

            builder.ConfigureBaseModel();
        }
    }
}
