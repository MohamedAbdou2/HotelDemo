using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelDemo.Migrations
{
    /// <inheritdoc />
    public partial class EditRerservationDiscount_PaymentUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Payments_PaymentId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_PaymentId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PaymentDate",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PaymentMethodId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Payments");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_ReservationStatusId",
                table: "Reservations",
                newName: "IX_Reservations_Status");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_PaymentStatusId",
                table: "Payments",
                newName: "IX_Payments_Status");

            migrationBuilder.AlterTable(
                name: "Payments",
                comment: "Payment transactions for reservations");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "Reservations",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "Reservations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckedInAt",
                table: "Reservations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckedOutAt",
                table: "Reservations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ConfirmedAt",
                table: "Reservations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "Reservations",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "Reservations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Reservations",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Payments",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "FailedAt",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FailureCode",
                table: "Payments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FailureReason",
                table: "Payments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GatewayResponse",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "Payments",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentUrl",
                table: "Payments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Payments",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "Payments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WebhookVerified",
                table: "Payments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ReservationCancellations",
                columns: table => new
                {
                    ReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CancelledByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CancellationType = table.Column<int>(type: "int", nullable: false, comment: "1=Auto, 2=UserRequested, 3=AdminCancelled"),
                    RefundProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationCancellations", x => x.ReservationId);
                    table.ForeignKey(
                        name: "FK_ReservationCancellations_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservationCancellations_Users_CancelledByUserId",
                        column: x => x.CancelledByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Stores detailed cancellation information for reservations");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_CheckInDate",
                table: "Reservations",
                column: "CheckInDate");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_CheckOutDate",
                table: "Reservations",
                column: "CheckOutDate");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ExpiresAt",
                table: "Reservations",
                column: "ExpiresAt",
                filter: "[ExpiresAt] IS NOT NULL AND [ReservationStatusId] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_Room_Dates",
                table: "Reservations",
                columns: new[] { "RoomId", "CheckInDate", "CheckOutDate" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Reservations_ExpiresAt_Valid",
                table: "Reservations",
                sql: "[ExpiresAt] IS NULL OR [ExpiresAt] > [CreatedAt]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Reservations_TotalPrice_Positive",
                table: "Reservations",
                sql: "[TotalPrice] > 0");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CreatedAt",
                table: "Payments",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CustomerId",
                table: "Payments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_FailedAt",
                table: "Payments",
                column: "FailedAt",
                filter: "[FailedAt] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Method_Status",
                table: "Payments",
                columns: new[] { "PaymentMethodId", "PaymentStatusId" });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TransactionId",
                table: "Payments",
                column: "TransactionId",
                unique: true,
                filter: "[TransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_WebhookPending",
                table: "Payments",
                column: "WebhookVerified",
                filter: "[PaymentStatusId] = 1 AND [WebhookVerified] = 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Payments_Amount_Positive",
                table: "Payments",
                sql: "[Amount] > 0");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationCancellations_Date",
                table: "ReservationCancellations",
                column: "CancelledAt");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationCancellations_Type",
                table: "ReservationCancellations",
                column: "CancellationType");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationCancellations_UserId",
                table: "ReservationCancellations",
                column: "CancelledByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Customers_CustomerId",
                table: "Payments",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Reservations_ReservationId",
                table: "Payments",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Customers_CustomerId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Reservations_ReservationId",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "ReservationCancellations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_CheckInDate",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_CheckOutDate",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_ExpiresAt",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_Room_Dates",
                table: "Reservations");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Reservations_ExpiresAt_Valid",
                table: "Reservations");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Reservations_TotalPrice_Positive",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Payments_CreatedAt",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_CustomerId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_FailedAt",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_Method_Status",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_TransactionId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_WebhookPending",
                table: "Payments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Payments_Amount_Positive",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "CheckedInAt",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "CheckedOutAt",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ConfirmedAt",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "FailedAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "FailureCode",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "FailureReason",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "GatewayResponse",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentUrl",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "WebhookVerified",
                table: "Payments");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_Status",
                table: "Reservations",
                newName: "IX_Reservations_ReservationStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_Status",
                table: "Payments",
                newName: "IX_Payments_PaymentStatusId");

            migrationBuilder.AlterTable(
                name: "Payments",
                oldComment: "Payment transactions for reservations");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "Reservations",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentId",
                table: "Reservations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Payments",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "Payments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_PaymentId",
                table: "Reservations",
                column: "PaymentId",
                unique: true,
                filter: "[PaymentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentDate",
                table: "Payments",
                column: "PaymentDate");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentMethodId",
                table: "Payments",
                column: "PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Payments_PaymentId",
                table: "Reservations",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
