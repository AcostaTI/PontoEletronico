using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PontoEletronico.Infrastructure.Persistence.Migrations
{
    public partial class AdicionaIndiceRegistroPontoUsuarioData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "RegistrosPontos",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosPontos_UsuarioId_DataRegistro",
                table: "RegistrosPontos",
                columns: new[] { "UsuarioId", "DataRegistro" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RegistrosPontos_UsuarioId_DataRegistro",
                table: "RegistrosPontos");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "RegistrosPontos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
