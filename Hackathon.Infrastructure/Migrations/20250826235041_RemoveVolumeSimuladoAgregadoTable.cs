using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hackathon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVolumeSimuladoAgregadoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VolumeSimuladoAgregado",
                schema: "main");

            migrationBuilder.RenameTable(
                name: "SIMULACAO",
                schema: "main",
                newName: "SIMULACAO");

            migrationBuilder.RenameTable(
                name: "RESULTADO_SIMULACAO",
                schema: "main",
                newName: "RESULTADO_SIMULACAO");

            migrationBuilder.RenameTable(
                name: "PARCELA",
                schema: "main",
                newName: "PARCELA");

            migrationBuilder.RenameTable(
                name: "METRICA_REQUISICAO",
                schema: "main",
                newName: "METRICA_REQUISICAO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "main");

            migrationBuilder.RenameTable(
                name: "SIMULACAO",
                newName: "SIMULACAO",
                newSchema: "main");

            migrationBuilder.RenameTable(
                name: "RESULTADO_SIMULACAO",
                newName: "RESULTADO_SIMULACAO",
                newSchema: "main");

            migrationBuilder.RenameTable(
                name: "PARCELA",
                newName: "PARCELA",
                newSchema: "main");

            migrationBuilder.RenameTable(
                name: "METRICA_REQUISICAO",
                newName: "METRICA_REQUISICAO",
                newSchema: "main");

            migrationBuilder.CreateTable(
                name: "VolumeSimuladoAgregado",
                schema: "main",
                columns: table => new
                {
                    CodigoProduto = table.Column<int>(type: "INTEGER", nullable: false),
                    DescricaoProduto = table.Column<string>(type: "TEXT", nullable: false),
                    TaxaMediaJuro = table.Column<decimal>(type: "TEXT", nullable: false),
                    ValorMedioPrestacao = table.Column<decimal>(type: "TEXT", nullable: false),
                    ValorTotalCredito = table.Column<decimal>(type: "TEXT", nullable: false),
                    ValorTotalDesejado = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                });
        }
    }
}
