using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace booking_system.Migrations
{
    /// <inheritdoc />
    public partial class bookingId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_ClassBookings_ClassBookingId",
                table: "Refunds");

            migrationBuilder.RenameColumn(
                name: "ClassBookingId",
                table: "Refunds",
                newName: "BookingId");

            migrationBuilder.RenameIndex(
                name: "IX_Refunds_ClassBookingId",
                table: "Refunds",
                newName: "IX_Refunds_BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_ClassBookings_BookingId",
                table: "Refunds",
                column: "BookingId",
                principalTable: "ClassBookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_ClassBookings_BookingId",
                table: "Refunds");

            migrationBuilder.RenameColumn(
                name: "BookingId",
                table: "Refunds",
                newName: "ClassBookingId");

            migrationBuilder.RenameIndex(
                name: "IX_Refunds_BookingId",
                table: "Refunds",
                newName: "IX_Refunds_ClassBookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_ClassBookings_ClassBookingId",
                table: "Refunds",
                column: "ClassBookingId",
                principalTable: "ClassBookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
