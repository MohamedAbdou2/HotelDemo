using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelDemo.Persistence.Infrastructure.EntitiesConfigurations
{
    public class StaffConfiguration : IEntityTypeConfiguration<Staff>
    {
        public void Configure(EntityTypeBuilder<Staff> builder)
        {
            builder.ToTable("Staff");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.UserId)
                   .IsUnique();

            builder.Property(x => x.Position)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.HireDate)
                   .IsRequired();

            builder.Property(x => x.TerminationDate)
                   .IsRequired(false);

            builder.HasOne(x => x.User)
                   .WithMany()
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.ConfigureBaseModel();
        }
    }
}

