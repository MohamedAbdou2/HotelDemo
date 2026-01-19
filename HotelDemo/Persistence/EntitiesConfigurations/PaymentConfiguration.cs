using HotelDemo.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelDemo.Persistence.EntitiesConfigurations
{
    public class PaymentConfiguration
        : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                   .HasPrecision(10, 2)
                   .IsRequired();

            builder.Property(x => x.PaymentDate)
                   .IsRequired();

            builder.HasOne(x => x.Reservation)
                   .WithOne(r => r.Payment)
                   .HasForeignKey<Reservation>(x => x.PaymentId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x => x.PaymentMethod)
                   .WithMany()
                   .HasForeignKey(x => x.PaymentMethodId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.PaymentMethodId)
                   .HasConversion<int>()
                   .IsRequired();

            builder.HasOne(x => x.PaymentStatus)
                   .WithMany()
                   .HasForeignKey(x => x.PaymentStatusId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.PaymentStatusId)
                   .HasConversion<int>()
                   .IsRequired();

            builder.HasIndex(x => x.ReservationId);
            builder.HasIndex(x => x.PaymentDate);

            builder.ConfigureBaseModel();
        }
    }
}
