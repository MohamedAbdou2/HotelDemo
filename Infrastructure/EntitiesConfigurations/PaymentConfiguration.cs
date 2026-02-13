using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelDemo.Persistence.Infrastructure.EntitiesConfigurations
{
    public class PaymentConfiguration
        : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
           // ==================== Table & Constraints ====================


            builder.ToTable("Payments", t =>
            {
                t.HasComment("Payment transactions for reservations");

                // Amount must be positive
                t.HasCheckConstraint(
                    "CK_Payments_Amount_Positive",
                    "[Amount] > 0"
                );

            

            });

            // ==================== Primary Key ====================

            builder.HasKey(x => x.Id);

            // ==================== Properties ====================

            builder.Property(x => x.ReservationId)
                   .IsRequired();

            builder.Property(x => x.CustomerId)
                   .IsRequired();

            builder.Property(x => x.Amount)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(x => x.PaymentMethodId)
                   .IsRequired()
                   .HasConversion<int>();

            builder.Property(x => x.PaymentStatusId)
                   .IsRequired()
                   .HasConversion<int>();

            builder.Property(x => x.TransactionId)
                   .HasMaxLength(200);

            builder.Property(x => x.GatewayResponse)
                   .HasColumnType("nvarchar(max)");

            builder.Property(x => x.IpAddress)
                   .HasMaxLength(45);

            builder.Property(x => x.WebhookVerified)
                   .IsRequired()
                   .HasDefaultValue(false);

            // Timestamps
            builder.Property(x => x.CompletedAt)
                   .HasColumnType("datetime2");

            builder.Property(x => x.FailedAt)
                   .HasColumnType("datetime2");

        

            // Failure Info
            builder.Property(x => x.FailureReason)
                   .HasMaxLength(500);

            builder.Property(x => x.FailureCode)
                   .HasMaxLength(100);

          

            // ==================== Concurrency Control ====================

            builder.Property(x => x.RowVersion)
                   .IsRowVersion()
                   .IsConcurrencyToken();

            // ==================== Indexes ====================

            // Index for finding payments by reservation
            builder.HasIndex(x => x.ReservationId)
                   .HasDatabaseName("IX_Payments_ReservationId");

            // Index for finding payments by customer
            builder.HasIndex(x => x.CustomerId)
                   .HasDatabaseName("IX_Payments_CustomerId");

            // Unique index for TransactionId (prevent duplicates)
            builder.HasIndex(x => x.TransactionId)
                   .IsUnique()
                   .HasDatabaseName("IX_Payments_TransactionId")
                   .HasFilter("[TransactionId] IS NOT NULL");

            // Index for finding payments by status
            builder.HasIndex(x => x.PaymentStatusId)
                   .HasDatabaseName("IX_Payments_Status");

            // Index for date-based queries
            builder.HasIndex(x => x.CreatedAt)
                   .HasDatabaseName("IX_Payments_CreatedAt");

            // Composite index for payment method analytics
            builder.HasIndex(x => new { x.PaymentMethodId, x.PaymentStatusId })
                   .HasDatabaseName("IX_Payments_Method_Status");

            // Filtered index for pending payments (webhook processing)
            builder.HasIndex(x => x.WebhookVerified)
                   .HasDatabaseName("IX_Payments_WebhookPending")
                   .HasFilter("[PaymentStatusId] = 1 AND [WebhookVerified] = 0"); // Pending & not verified

            // Filtered index for failed payments (retry analysis)
            builder.HasIndex(x => x.FailedAt)
                   .HasDatabaseName("IX_Payments_FailedAt")
                   .HasFilter("[FailedAt] IS NOT NULL");

            // ==================== Relationships ====================

            // Payment -> Reservation (Many-to-One)
            builder.HasOne(x => x.Reservation)
                   .WithMany(r => r.Payments)
                   .HasForeignKey(x => x.ReservationId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Payment -> Customer (Many-to-One)
            builder.HasOne(x => x.Customer)
                   .WithMany(c => c.Payments)
                   .HasForeignKey(x => x.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Payment -> PaymentMethod (Many-to-One)
            builder.HasOne(x => x.PaymentMethod)
                   .WithMany()
                   .HasForeignKey(x => x.PaymentMethodId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Payment -> PaymentStatus (Many-to-One)
            builder.HasOne(x => x.PaymentStatus)
                   .WithMany()
                   .HasForeignKey(x => x.PaymentStatusId)
                   .OnDelete(DeleteBehavior.Restrict);

            // ==================== Base Model Configuration ====================

            builder.ConfigureBaseModel();

        }
    }
}
