using HotelDemo.Data.Enums;
using HotelDemo.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelDemo.Persistence.EntitiesConfigurations
{
    public class ReservationStatusConfiguration : IEntityTypeConfiguration<ReservationStatus>
    {
        public void Configure(EntityTypeBuilder<ReservationStatus> builder)
        {
            builder.ToTable("ReservationStatuses");

            builder.HasKey(rs => rs.Id);
            builder.Property(rs => rs.Id)
                .HasConversion<int>()
                .ValueGeneratedNever();
            builder.Property(rs => rs.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(rs => rs.IsAvailable)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.HasData(
                new ReservationStatus { Id = ReservationStatusCode.Pending, Name = "Pending",IsAvailable = true},
                new ReservationStatus { Id = ReservationStatusCode.Confirmed, Name = "Confirmed" , IsAvailable = true},
                new ReservationStatus { Id = ReservationStatusCode.CheckedIn, Name = "CheckedIn" , IsAvailable = true },
                new ReservationStatus { Id = ReservationStatusCode.CheckedOut, Name = "CheckedOut" , IsAvailable = true},
                new ReservationStatus { Id = ReservationStatusCode.Cancelled, Name = "Cancelled" , IsAvailable = true }
            );
        }

    }
}
