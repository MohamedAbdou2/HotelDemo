using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelDemo.Persistence.Infrastructure.EntitiesConfigurations
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            // ==================== Table & Constraints ====================

            builder.ToTable("Reservations", t =>
            {
                // Check constraint: CheckOut must be after CheckIn
                t.HasCheckConstraint(
                    "CK_Reservations_CheckOut_After_CheckIn",
                    "[CheckOutDate] > [CheckInDate]"
                );

                // Check constraint: TotalPrice must be positive
                t.HasCheckConstraint(
                    "CK_Reservations_TotalPrice_Positive",
                    "[TotalPrice] > 0"
                );

                // Check constraint: ExpiresAt must be after creation
                t.HasCheckConstraint(
                    "CK_Reservations_ExpiresAt_Valid",
                    "[ExpiresAt] IS NULL OR [ExpiresAt] > [CreatedAt]"
                );
            });

            // ==================== Primary Key ====================

            builder.HasKey(x => x.Id);

            // ==================== Properties ====================

            builder.Property(x => x.RoomId)
                   .IsRequired();

            builder.Property(x => x.CustomerId)
                   .IsRequired();

            builder.Property(x => x.CheckInDate)
                   .IsRequired()
                   .HasColumnType("datetime2");

            builder.Property(x => x.CheckOutDate)
                   .IsRequired()
                   .HasColumnType("datetime2");

            builder.Property(x => x.TotalPrice)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(x => x.ReservationStatusId)
                   .IsRequired()
                   .HasConversion<int>();

            // ==================== Timestamps ====================

            builder.Property(x => x.ExpiresAt)
                   .HasColumnType("datetime2");

            builder.Property(x => x.ConfirmedAt)
                   .HasColumnType("datetime2");

            builder.Property(x => x.CancelledAt)
                   .HasColumnType("datetime2");

            builder.Property(x => x.CheckedInAt)
                   .HasColumnType("datetime2");

            builder.Property(x => x.CheckedOutAt)
                   .HasColumnType("datetime2");


            // ==================== Concurrency Control ====================

            builder.Property(x => x.RowVersion)
                   .IsRowVersion()
                   .IsConcurrencyToken();

            // ==================== Indexes ====================

            // Index for finding reservations by customer
            builder.HasIndex(x => x.CustomerId)
                   .HasDatabaseName("IX_Reservations_CustomerId");

            // Index for finding reservations by room
            builder.HasIndex(x => x.RoomId)
                   .HasDatabaseName("IX_Reservations_RoomId");

            // Index for finding reservations by status
            builder.HasIndex(x => x.ReservationStatusId)
                   .HasDatabaseName("IX_Reservations_Status");

            // Composite index for checking room availability by date range
            builder.HasIndex(x => new { x.RoomId, x.CheckInDate, x.CheckOutDate })
                   .HasDatabaseName("IX_Reservations_Room_Dates");

            // Filtered index for pending reservations with expiration
            builder.HasIndex(x => x.ExpiresAt)
                   .HasDatabaseName("IX_Reservations_ExpiresAt")
                   .HasFilter("[ExpiresAt] IS NOT NULL AND [ReservationStatusId] = 1"); // Pending only

            // Index for finding reservations by date range
            builder.HasIndex(x => x.CheckInDate)
                   .HasDatabaseName("IX_Reservations_CheckInDate");

            builder.HasIndex(x => x.CheckOutDate)
                   .HasDatabaseName("IX_Reservations_CheckOutDate");

            // ==================== Relationships ====================

            // Reservation -> Customer (Many-to-One)
            builder.HasOne(x => x.Customer)
                   .WithMany(c => c.Reservations)
                   .HasForeignKey(x => x.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Reservation -> Room (Many-to-One)
            builder.HasOne(x => x.Room)
                   .WithMany(r => r.Reservations)
                   .HasForeignKey(x => x.RoomId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Reservation -> ReservationStatus (Many-to-One)
            builder.HasOne(x => x.ReservationStatus)
                   .WithMany()
                   .HasForeignKey(x => x.ReservationStatusId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Reservation -> ReservationCancellation (One-to-Zero-or-One)
            builder.HasOne(x => x.Cancellation)
                   .WithOne(c => c.Reservation)
                   .HasForeignKey<ReservationCancellation>(c => c.ReservationId)
                   .OnDelete(DeleteBehavior.Cascade); // When reservation deleted, delete cancellation

            // Reservation -> Payments (One-to-Many)
            builder.HasMany(x => x.Payments)
                   .WithOne(p => p.Reservation)
                   .HasForeignKey(p => p.ReservationId)
                   .OnDelete(DeleteBehavior.Restrict);

            // ==================== Base Model Configuration ====================

            builder.ConfigureBaseModel();

          
        }
    }
}
