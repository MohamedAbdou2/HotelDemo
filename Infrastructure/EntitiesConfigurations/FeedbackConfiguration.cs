using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelDemo.Persistence.Infrastructure.EntitiesConfigurations
{
    public class FeedbackConfiguration
        : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Rating)
                   .IsRequired();

            builder.Property(x => x.Comments)
                   .HasMaxLength(2000);

            builder.HasOne(x => x.Customer)
                   .WithMany(c => c.Feedbacks)
                   .HasForeignKey(x => x.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Reservation)
                   .WithMany()
                   .HasForeignKey(x => x.ReservationId)
                   .OnDelete(DeleteBehavior.NoAction);

                builder.Property(x => x.Rating)
                       .HasColumnType("decimal(2,1)") 
                       .IsRequired();

            //builder.HasOne(x => x.FeedbackResponse)
            //       .WithOne(fr => fr.Feedback)
            //       .HasForeignKey<FeedbackResponse>(fr => fr.FeedbackId)
            //       .OnDelete(DeleteBehavior.Cascade);

            builder.ConfigureBaseModel();
        }
    }
}
