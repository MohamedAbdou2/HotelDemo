
using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelDemo.Persistence.Infrastructure.EntitiesConfigurations
{
    public class ReportTypeConfiguration
        : IEntityTypeConfiguration<ReportType>
    {
        public void Configure(EntityTypeBuilder<ReportType> builder)
        {
            builder.ToTable("ReportTypes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .HasConversion<int>()
                   .ValueGeneratedNever();

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(x => x.IsAvailable)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.HasData(
                new ReportType
                {
                    Id = ReportTypeCode.Reservations,
                    Name = "Reservations"
                },
                new ReportType
                {
                    Id = ReportTypeCode.Revenue,
                    Name = "Revenue"
                },
                new ReportType
                {
                    Id = ReportTypeCode.CustomerDemographics,
                    Name = "Customer Demographics"
                }
            );
        }
    }
}
