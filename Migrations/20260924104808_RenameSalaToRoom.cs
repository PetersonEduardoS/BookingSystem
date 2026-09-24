using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSystem.Migrations
{
    /// <summary>
    /// Renames the Salas table to Rooms (and its columns to English) while keeping existing data.
    /// Hand-edited: EF scaffolded a drop/create, which would have deleted every room.
    /// </summary>
    public partial class RenameSalaToRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Salas_SalaId",
                table: "Reservas");

            migrationBuilder.RenameTable(
                name: "Salas",
                newName: "Rooms");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Rooms",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Capacidade",
                table: "Rooms",
                newName: "Capacity");

            migrationBuilder.RenameColumn(
                name: "SalaId",
                table: "Reservas",
                newName: "RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservas_SalaId",
                table: "Reservas",
                newName: "IX_Reservas_RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Rooms_RoomId",
                table: "Reservas",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Rooms_RoomId",
                table: "Reservas");

            migrationBuilder.RenameIndex(
                name: "IX_Reservas_RoomId",
                table: "Reservas",
                newName: "IX_Reservas_SalaId");

            migrationBuilder.RenameColumn(
                name: "RoomId",
                table: "Reservas",
                newName: "SalaId");

            migrationBuilder.RenameColumn(
                name: "Capacity",
                table: "Rooms",
                newName: "Capacidade");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Rooms",
                newName: "Nome");

            migrationBuilder.RenameTable(
                name: "Rooms",
                newName: "Salas");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Salas_SalaId",
                table: "Reservas",
                column: "SalaId",
                principalTable: "Salas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
