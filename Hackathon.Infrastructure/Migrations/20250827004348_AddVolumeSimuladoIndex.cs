using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hackathon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVolumeSimuladoIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SIMULACAO_DATA_PRODUTO",
                table: "SIMULACAO",
                columns: new[] { "DT_REFERENCIA", "CO_PRODUTO" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SIMULACAO_DATA_PRODUTO",
                table: "SIMULACAO");
        }
    }
}
