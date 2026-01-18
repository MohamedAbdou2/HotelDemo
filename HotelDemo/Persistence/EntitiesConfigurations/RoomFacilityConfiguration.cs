using HotelDemo.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelDemo.Persistence.EntitiesConfigurations
{
    public class RoomFacilityConfiguration : IEntityTypeConfiguration<RoomFacility>
    {
        public void Configure(EntityTypeBuilder<RoomFacility> builder)
        {
            builder.ToTable("RoomFacilities");

            builder.HasKey(x => new { x.RoomId, x.FacilityId });

            builder.Property(x => x.RoomId)
                   .IsRequired();

            builder.Property(x => x.FacilityId)
                   .HasConversion<int>()
                   .IsRequired();

        }
    }
}
