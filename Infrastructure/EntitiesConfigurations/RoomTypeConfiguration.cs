
using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelDemo.Persistence.Infrastructure.EntitiesConfigurations
{
    public class RoomTypeConfiguration : IEntityTypeConfiguration<RoomType>
    {
        public void Configure(EntityTypeBuilder<RoomType> builder)
        {
            builder.ToTable("RoomTypes");
            builder.HasKey(rt => rt.Id);
            builder.Property(x => x.Id)
                   .HasConversion<int>()
                   .ValueGeneratedNever();
            builder.Property(rt => rt.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(rt => rt.Description)
                .IsRequired()
                .HasMaxLength(500);
            builder.Property(rt => rt.PriceMultiplier)
                .IsRequired()
                .HasPrecision(5, 2);

            builder.Property(rt => rt.IsAvailable)
                .IsRequired()
                .HasDefaultValue(true);
            builder.HasData(
                new RoomType
                {
                    Id = RoomTypeCode.Single,
                    Name = "Single Room",
                    Description = "Single bed room",
                    PriceMultiplier = 1.0m
                },
                new RoomType
                {
                    Id = RoomTypeCode.Double,
                    Name = "Double Room",
                    Description = "Double bed room",
                    PriceMultiplier = 1.3m
                },
                new RoomType
                {
                    Id = RoomTypeCode.Suite,
                    Name = "Suite",
                    Description = "Luxury suite",
                    PriceMultiplier = 1.8m
                },
                new RoomType
                {
                    Id = RoomTypeCode.Deluxe,
                    Name = "Deluxe Room",
                    Description = "Premium deluxe room",
                    PriceMultiplier = 2.2m
                }
            );

        }
    }
}
