using HotelDemo.Data.Enums;
using HotelDemo.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelDemo.Persistence.EntitiesConfigurations
{
    public class PaymentStatusConfiguration
        : IEntityTypeConfiguration<PaymentStatus>
    {
        public void Configure(EntityTypeBuilder<PaymentStatus> builder)
        {
            builder.ToTable("PaymentStatuses");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .HasConversion<int>()
                   .ValueGeneratedNever();

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x=> x.IsAvailable)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.HasData(
                new PaymentStatus
                {
                    Id = PaymentStatusCode.Pending,
                    Name = "Pending"
                },
                new PaymentStatus
                {
                    Id = PaymentStatusCode.Paid,
                    Name = "Paid"
                },
                new PaymentStatus
                {
                    Id = PaymentStatusCode.Failed,
                    Name = "Failed"
                },
                new PaymentStatus
                {
                    Id = PaymentStatusCode.Refunded,
                    Name = "Refunded"
                }
            );
        }
    }
}
