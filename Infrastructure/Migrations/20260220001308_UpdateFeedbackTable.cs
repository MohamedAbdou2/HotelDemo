using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelDemo.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFeedbackTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Feedbacks_Rating_Range",
                table: "Feedbacks");

            migrationBuilder.AlterColumn<decimal>(
                name: "Rating",
                table: "Feedbacks",
                type: "decimal(2,1)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Rating",
                table: "Feedbacks",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,1)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Feedbacks_Rating_Range",
                table: "Feedbacks",
                sql: "[Rating] >= 1 AND [Rating] <= 5");
        }
    }
}
