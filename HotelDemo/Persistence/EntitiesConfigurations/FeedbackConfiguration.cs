using HotelDemo.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelDemo.Persistence.EntitiesConfigurations
{
    public class FeedbackConfiguration
        : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.ToTable("Feedbacks", t =>
            {
                t.HasCheckConstraint(
                "CK_Feedbacks_Rating_Range",
                "[Rating] >= 1 AND [Rating] <= 5"
            );
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Rating)
                   .IsRequired();

            builder.Property(x => x.Comments)
                   .IsRequired()
                   .HasMaxLength(2000);

            builder.HasOne(x => x.Customer)
                   .WithMany(c => c.Feedbacks)
                   .HasForeignKey(x => x.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Room)
                   .WithMany(r => r.Feedbacks)
                   .HasForeignKey(x => x.RoomId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.FeedbackResponse)
                   .WithOne(fr => fr.Feedback)
                   .HasForeignKey<FeedbackResponse>(fr => fr.FeedbackId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.ConfigureBaseModel();
        }
    }
}
