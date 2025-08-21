using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hackathon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SIMULACAO",
                columns: table => new
                {
                    ID_SIMULACAO = table.Column<Guid>(type: "TEXT", nullable: false),
                    CO_PRODUTO = table.Column<int>(type: "INTEGER", nullable: false),
                    NO_PRODUTO = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PC_TAXA_JUROS = table.Column<decimal>(type: "decimal(10,9)", nullable: false),
                    VR_DESEJADO = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NU_PRAZO_MESES = table.Column<short>(type: "INTEGER", nullable: false),
                    DT_REFERENCIA = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    TX_ENVELOP_JSON = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIMULACAO", x => x.ID_SIMULACAO);
                });

            migrationBuilder.CreateTable(
                name: "RESULTADO_SIMULACAO",
                columns: table => new
                {
                    ID_RESULTADO = table.Column<Guid>(type: "TEXT", nullable: false),
                    ID_SIMULACAO = table.Column<Guid>(type: "TEXT", nullable: false),
                    TP_SISTEMA_AMORTIZACAO = table.Column<int>(type: "INTEGER", nullable: false),
                    VR_TOTAL = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RESULTADO_SIMULACAO", x => x.ID_RESULTADO);
                    table.ForeignKey(
                        name: "FK_RESULTADO_SIMULACAO_SIMULACAO_ID_SIMULACAO",
                        column: x => x.ID_SIMULACAO,
                        principalTable: "SIMULACAO",
                        principalColumn: "ID_SIMULACAO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PARCELA",
                columns: table => new
                {
                    ID_RESULTADO = table.Column<Guid>(type: "TEXT", nullable: false),
                    NU_PARCELA = table.Column<int>(type: "INTEGER", nullable: false),
                    VR_PRESTACAO = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VR_AMORTIZACAO = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VR_JUROS = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PARCELA", x => new { x.ID_RESULTADO, x.NU_PARCELA });
                    table.ForeignKey(
                        name: "FK_PARCELA_RESULTADO_SIMULACAO_ID_RESULTADO",
                        column: x => x.ID_RESULTADO,
                        principalTable: "RESULTADO_SIMULACAO",
                        principalColumn: "ID_RESULTADO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PARCELA_NU_PARCELA",
                table: "PARCELA",
                column: "NU_PARCELA");

            migrationBuilder.CreateIndex(
                name: "IX_RESULTADO_SIMULACAO_ID_SIMULACAO",
                table: "RESULTADO_SIMULACAO",
                column: "ID_SIMULACAO");

            migrationBuilder.CreateIndex(
                name: "IX_SIMULACAO_CO_PRODUTO",
                table: "SIMULACAO",
                column: "CO_PRODUTO");

            migrationBuilder.CreateIndex(
                name: "IX_SIMULACAO_DT_REFERENCIA",
                table: "SIMULACAO",
                column: "DT_REFERENCIA");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PARCELA");

            migrationBuilder.DropTable(
                name: "RESULTADO_SIMULACAO");

            migrationBuilder.DropTable(
                name: "SIMULACAO");
        }
    }
}
