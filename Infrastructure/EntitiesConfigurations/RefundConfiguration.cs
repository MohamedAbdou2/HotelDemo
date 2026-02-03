using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfigurations
{
    public class RefundConfiguration : IEntityTypeConfiguration<Refund>
    {
        public void Configure(EntityTypeBuilder<Refund> builder)
        {
            builder.ToTable("Refunds");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(r => r.Reason)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(r => r.GatewayRefundId)
                .HasMaxLength(200);

            builder.Property(r => r.FailureReason)
                .HasMaxLength(500);

            builder.HasOne(r => r.Payment)
                .WithMany(p => p.Refunds)
                .HasForeignKey(r => r.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.ProcessedByUser)
                .WithMany()
                .HasForeignKey(r => r.ProcessedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(r => r.PaymentId);
            builder.HasIndex(r => r.RefundStatusId);
            builder.HasIndex(r => r.CreatedAt);
        }
    }
}
