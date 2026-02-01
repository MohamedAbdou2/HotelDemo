using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelDemo.Persistence.Infrastructure.EntitiesConfigurations
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.ToTable("Reservations", t =>
            {
                t.HasCheckConstraint(
                    "CK_Reservations_CheckOut_After_CheckIn",
                    "[CheckOutDate] > [CheckInDate]"
                );

            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CheckInDate)
                   .IsRequired();

            builder.Property(x => x.CheckOutDate)
                   .IsRequired();

            builder.Property(x => x.TotalPrice)
                   .HasPrecision(10, 2)
                   .IsRequired();


            builder.HasOne(x => x.Customer)
                   .WithMany(c => c.Reservations)
                   .HasForeignKey(x => x.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.ConfigureBaseModel();
        }
    }
}
