using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelDemo.Persistence.Infrastructure.EntitiesConfigurations
{
    public class ReservationCancellationConfiguration : IEntityTypeConfiguration<ReservationCancellation>
    {
        public void Configure(EntityTypeBuilder<ReservationCancellation> builder)
        {
            // ==================== Table ====================

            builder.ToTable("ReservationCancellations", t =>
            {
                // Comment for documentation
                t.HasComment("Stores detailed cancellation information for reservations");
            });

            // ==================== Primary Key ====================

            // ReservationId is both PK and FK (1:1 relationship)
            builder.HasKey(x => x.ReservationId);

            // ==================== Properties ====================

            builder.Property(x => x.ReservationId)
                   .IsRequired();

            builder.Property(x => x.Reason)
                   .IsRequired()
                   .HasMaxLength(500)
                   .IsUnicode(true);

            builder.Property(x => x.CancelledByUserId)
                   .IsRequired();

            builder.Property(x => x.CancelledAt)
                   .IsRequired()
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(x => x.CancellationType)
                   .IsRequired()
                   .HasConversion<int>()
                   .HasComment("1=Auto, 2=UserRequested, 3=AdminCancelled");

 

            builder.Property(x => x.RefundProcessedAt)
                   .HasColumnType("datetime2");

            // ==================== Indexes ====================

            // Index for finding cancellations by user
            builder.HasIndex(x => x.CancelledByUserId)
                   .HasDatabaseName("IX_ReservationCancellations_UserId");

            // Index for finding cancellations by date
            builder.HasIndex(x => x.CancelledAt)
                   .HasDatabaseName("IX_ReservationCancellations_Date");

            // Index for finding cancellations by type
            builder.HasIndex(x => x.CancellationType)
                   .HasDatabaseName("IX_ReservationCancellations_Type");

            
        

            // ==================== Relationships ====================

            // ReservationCancellation -> Reservation (One-to-One)
            // Configured in ReservationConfiguration

            // ReservationCancellation -> User (CancelledBy)
            builder.HasOne(x => x.CancelledByUser)
                   .WithMany()
                   .HasForeignKey(x => x.CancelledByUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}