using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using HotelDemo.Persistence.Infrastructure.EntitiesConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfigurations
{
    public class UserOtpConfiguration : IEntityTypeConfiguration<UserOtp>
    {
        public void Configure(EntityTypeBuilder<UserOtp> builder)
        {
            builder.ToTable("UserOtp");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.otp)
                .IsRequired()
                .HasMaxLength(6);

            builder.Property(x => x.ExpiresAt)
                .IsRequired();

            builder.ConfigureBaseModel();

        }
    }
}
