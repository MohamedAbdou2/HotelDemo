using HotelDemo.Data.Enums;
using HotelDemo.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelDemo.Persistence.EntitiesConfigurations
{
    public class FacilityConfiguration : IEntityTypeConfiguration<Facility>
    {
        public void Configure(EntityTypeBuilder<Facility> builder)
        {
            builder.ToTable("Facilities");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .HasConversion<int>()
                   .ValueGeneratedNever();

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.Description)
                   .HasMaxLength(500);

            builder.Property(x=>x.IsAvailable)
                   .HasDefaultValue(true);

            builder.HasMany(r => r.RoomFacilities)
                   .WithOne(rf => rf.Facility)
                   .HasForeignKey(rf => rf.FacilityId);

            builder.HasData(
                new Facility
                {
                    Id = RoomFacilityCode.Wifi,
                    Name = "Wi-Fi",
                    Description = "High-speed wireless internet access"
                },
                new Facility
                {
                    Id = RoomFacilityCode.AirConditioning,
                    Name = "Air Conditioning",
                    Description = "Individually controlled air conditioning"
                },
                new Facility
                {
                    Id = RoomFacilityCode.BreakfastIncluded,
                    Name = "Breakfast Included",
                    Description = "Daily breakfast included with the room"
                },
                new Facility
                {
                    Id = RoomFacilityCode.SwimmingPoolAccess,
                    Name = "Swimming Pool Access",
                    Description = "Access to the hotel swimming pool"
                },
                new Facility
                {
                    Id = RoomFacilityCode.GymAccess,
                    Name = "Gym Access",
                    Description = "Access to the fitness center"
                }
            );

        }

    }
}
