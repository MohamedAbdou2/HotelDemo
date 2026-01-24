
using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelDemo.Persistence.Infrastructure.EntitiesConfigurations
{
    public class ReportPeriodConfiguration
        : IEntityTypeConfiguration<ReportPeriod>
    {
        public void Configure(EntityTypeBuilder<ReportPeriod> builder)
        {
            builder.ToTable("ReportPeriods");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .HasConversion<int>()
                   .ValueGeneratedNever();

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.IsAvailable)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.HasData(
                new ReportPeriod
                {
                    Id = ReportPeriodCode.Daily,
                    Name = "Daily"
                },
                new ReportPeriod
                {
                    Id = ReportPeriodCode.Weekly,
                    Name = "Weekly"
                },
                new ReportPeriod
                {
                    Id = ReportPeriodCode.Monthly,
                    Name = "Monthly"
                },
                new ReportPeriod
                {
                    Id = ReportPeriodCode.Yearly,
                    Name = "Yearly"
                }
            );
        }
    }
}
