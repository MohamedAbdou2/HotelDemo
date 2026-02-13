using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelDemo.Migrations
{
    /// <inheritdoc />
    public partial class UpateFeedbackTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Rooms_RoomId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_FeedbackResponses_FeedbackId",
                table: "FeedbackResponses");

            migrationBuilder.RenameColumn(
                name: "RoomId",
                table: "Feedbacks",
                newName: "ReservationId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_RoomId",
                table: "Feedbacks",
                newName: "IX_Feedbacks_ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackResponses_FeedbackId",
                table: "FeedbackResponses",
                column: "FeedbackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Rooms_ReservationId",
                table: "Feedbacks",
                column: "ReservationId",
                principalTable: "Rooms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Rooms_ReservationId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_FeedbackResponses_FeedbackId",
                table: "FeedbackResponses");

            migrationBuilder.RenameColumn(
                name: "ReservationId",
                table: "Feedbacks",
                newName: "RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_ReservationId",
                table: "Feedbacks",
                newName: "IX_Feedbacks_RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackResponses_FeedbackId",
                table: "FeedbackResponses",
                column: "FeedbackId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Rooms_RoomId",
                table: "Feedbacks",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");
        }
    }
}
