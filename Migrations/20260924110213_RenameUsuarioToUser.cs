using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSystem.Migrations
{
    /// <summary>
    /// Renames the Usuarios table to Users (and its columns to English) while keeping existing data,
    /// and converts the "usuario" role to "user".
    /// Hand-edited: EF scaffolded a drop/create, which would have deleted every user.
    /// </summary>
    public partial class RenameUsuarioToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Usuarios_UsuarioId",
                table: "Reservas");

            migrationBuilder.RenameTable(
                name: "Usuarios",
                newName: "Users");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Users",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "SenhaHash",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.Sql("UPDATE \"Users\" SET \"Role\" = 'user' WHERE \"Role\" = 'usuario';");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Reservas",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservas_UsuarioId",
                table: "Reservas",
                newName: "IX_Reservas_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Users_UserId",
                table: "Reservas",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Users_UserId",
                table: "Reservas");

            migrationBuilder.RenameIndex(
                name: "IX_Reservas_UserId",
                table: "Reservas",
                newName: "IX_Reservas_UsuarioId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Reservas",
                newName: "UsuarioId");

            migrationBuilder.Sql("UPDATE \"Users\" SET \"Role\" = 'usuario' WHERE \"Role\" = 'user';");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Users",
                newName: "SenhaHash");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Users",
                newName: "Nome");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Usuarios");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Usuarios_UsuarioId",
                table: "Reservas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
