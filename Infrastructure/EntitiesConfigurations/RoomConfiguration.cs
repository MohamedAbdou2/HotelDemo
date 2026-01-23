using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelDemo.Persistence.Infrastructure.EntitiesConfigurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.ToTable("Rooms");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RoomNumber)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(x => x.PricePerNight)
                   .HasPrecision(10, 2);

            builder.Property(x => x.IsAvailable)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.HasMany(r => r.RoomPictures)
                   .WithOne(rp => rp.Room)
                   .HasForeignKey(rp => rp.RoomId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(r => r.RoomFacilities)
                   .WithOne(rf => rf.Room)
                   .HasForeignKey(rf => rf.RoomId)
                   .OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(r => r.Reservations)
                   .WithOne(res => res.Room)
                   .HasForeignKey(res => res.RoomId)
                   .OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(r => r.RoomOffers)
                   .WithOne(ro => ro.Room)
                   .HasForeignKey(ro => ro.RoomId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.Property(x => x.RoomTypeId)
                    .HasConversion<int>()
                    .IsRequired();
            builder.HasOne(r => r.Type)
                     .WithMany(rt => rt.Rooms)
                     .HasForeignKey(r => r.RoomTypeId)
                     .OnDelete(DeleteBehavior.Restrict);

            builder.ConfigureBaseModel();
        }
    }
}
