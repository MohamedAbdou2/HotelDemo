using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelDemo.Persistence.Infrastructure.EntitiesConfigurations
{
    public class FeedbackResponseConfiguration
        : IEntityTypeConfiguration<FeedbackResponse>
    {
        public void Configure(EntityTypeBuilder<FeedbackResponse> builder)
        {
            builder.ToTable("FeedbackResponses");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ResponseText)
                   .IsRequired()
                   .HasMaxLength(2000);

            builder.Property(x => x.ResponseDate)
                   .IsRequired();

            builder.Property(x => x.FeedbackId)
                   .IsRequired();

            builder.ConfigureBaseModel();
        }
    }
}
