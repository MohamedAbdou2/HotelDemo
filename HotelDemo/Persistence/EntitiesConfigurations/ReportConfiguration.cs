using HotelDemo.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelDemo.Persistence.EntitiesConfigurations
{
    public class ReportConfiguration
        : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.ToTable("Reports");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.Content)
                   .IsRequired();

            builder.HasOne(x => x.ReportType)
                   .WithMany()
                   .HasForeignKey(x => x.ReportTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.ReportTypeId)
                   .HasConversion<int>()
                   .IsRequired(false);

            builder.HasOne(x => x.ReportPeriod)
                   .WithMany()
                   .HasForeignKey(x => x.ReportPeriodId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.ReportPeriodId)
                   .HasConversion<int>()
                   .IsRequired(false);

            builder.HasIndex(x => x.ReportTypeId);
            builder.HasIndex(x => x.ReportPeriodId);
            builder.HasIndex(x => x.CreatedAt);

            builder.ConfigureBaseModel();
        }
    }
}
