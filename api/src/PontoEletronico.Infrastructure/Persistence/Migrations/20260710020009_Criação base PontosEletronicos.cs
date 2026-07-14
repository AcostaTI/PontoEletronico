using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PontoEletronico.Infrastructure.Persistence.Migrations
{
    public partial class CriaçãobasePontosEletronicos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegistrosPontos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataRegistro = table.Column<DateTime>(type: "date", nullable: false),
                    HoraRegistro = table.Column<TimeSpan>(type: "time", nullable: false),
                    TipoRegistro = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosPontos", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrosPontos");
        }
    }
}
