using HotelDemo.Data.Enums;
using HotelDemo.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelDemo.Persistence.EntitiesConfigurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .HasConversion<int>()
                   .ValueGeneratedNever();

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(x => x.Name)
                   .IsUnique();

            builder.HasMany(x => x.UserRoles)
                   .WithOne(ur => ur.Role)
                   .HasForeignKey(ur => ur.RoleId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                new Role
                {
                    Id = UserRoleCode.Admin,
                    Name = "Admin"
                },
                new Role
                {
                    Id = UserRoleCode.Staff,
                    Name = "Staff"
                },
                new Role
                {
                    Id = UserRoleCode.Customer,
                    Name = "Customer"
                },
                new Role
                {
                    Id = UserRoleCode.Guest,
                    Name = "Guest"
                }
            );
        }
    }
}

