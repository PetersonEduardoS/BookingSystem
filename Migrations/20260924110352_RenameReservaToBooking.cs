using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSystem.Migrations
{
    /// <summary>
    /// Renames the Reservas table to Bookings (and its columns to English) while keeping existing data.
    /// Hand-edited: EF scaffolded a drop/create, which would have deleted every booking.
    /// </summary>
    public partial class RenameReservaToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Rooms_RoomId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Users_UserId",
                table: "Reservas");

            migrationBuilder.RenameTable(
                name: "Reservas",
                newName: "Bookings");

            migrationBuilder.RenameColumn(
                name: "DataInicio",
                table: "Bookings",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "DataFim",
                table: "Bookings",
                newName: "EndDate");

            migrationBuilder.RenameIndex(
                name: "IX_Reservas_RoomId",
                table: "Bookings",
                newName: "IX_Bookings_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservas_UserId",
                table: "Bookings",
                newName: "IX_Bookings_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Rooms_RoomId",
                table: "Bookings",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Rooms_RoomId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                newName: "IX_Reservas_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_RoomId",
                table: "Bookings",
                newName: "IX_Reservas_RoomId");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Bookings",
                newName: "DataFim");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Bookings",
                newName: "DataInicio");

            migrationBuilder.RenameTable(
                name: "Bookings",
                newName: "Reservas");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Rooms_RoomId",
                table: "Reservas",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Users_UserId",
                table: "Reservas",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
