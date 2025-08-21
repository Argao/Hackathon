using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hackathon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTelemetriaMetricas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "METRICA_REQUISICAO",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NO_API = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TX_ENDPOINT = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    NU_TEMPO_RESPOSTA_MS = table.Column<long>(type: "INTEGER", nullable: false),
                    FL_SUCESSO = table.Column<bool>(type: "INTEGER", nullable: false),
                    NU_STATUS_CODE = table.Column<int>(type: "INTEGER", nullable: false),
                    DT_HORA = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TX_IP_CLIENTE = table.Column<string>(type: "TEXT", maxLength: 45, nullable: true),
                    TX_USER_AGENT = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_METRICA_REQUISICAO", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_METRICA_API_DATA",
                table: "METRICA_REQUISICAO",
                columns: new[] { "NO_API", "DT_HORA" });

            migrationBuilder.CreateIndex(
                name: "IX_METRICA_DT_HORA",
                table: "METRICA_REQUISICAO",
                column: "DT_HORA");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "METRICA_REQUISICAO");
        }
    }
}
