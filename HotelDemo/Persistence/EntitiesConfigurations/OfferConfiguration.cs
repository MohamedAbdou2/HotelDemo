using HotelDemo.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelDemo.Persistence.EntitiesConfigurations
{
    public class OfferConfiguration
        : IEntityTypeConfiguration<Offer>
    {
        public void Configure(EntityTypeBuilder<Offer> builder)
        {
            builder.ToTable("Offers", t =>
            {
                t.HasCheckConstraint(
                    "CK_Offers_ValidDateRange",
                    "[EndDate] > [StartDate]"
                );

                t.HasCheckConstraint(
                    "CK_Offers_Discount_Range",
                    "[DiscountPercentage] > 0 AND [DiscountPercentage] <= 100"
                );
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(x => x.Description)
                   .HasMaxLength(1000);

            builder.Property(x => x.DiscountPercentage)
                   .HasPrecision(5, 2)
                   .IsRequired();

            builder.Property(x => x.IsActive)
                     .IsRequired()
                        .HasDefaultValue(true);

            builder.Property(x => x.StartDate)
                   .IsRequired();

            builder.Property(x => x.EndDate)
                   .IsRequired();

            builder.HasMany(x => x.RoomOffers)
                   .WithOne(ro => ro.Offer)
                   .HasForeignKey(ro => ro.OfferId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.ConfigureBaseModel();
        }
    }
}
