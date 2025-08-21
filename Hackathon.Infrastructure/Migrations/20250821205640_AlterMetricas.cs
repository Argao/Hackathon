using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hackathon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterMetricas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TX_IP_CLIENTE",
                table: "METRICA_REQUISICAO");

            migrationBuilder.DropColumn(
                name: "TX_USER_AGENT",
                table: "METRICA_REQUISICAO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TX_IP_CLIENTE",
                table: "METRICA_REQUISICAO",
                type: "TEXT",
                maxLength: 45,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TX_USER_AGENT",
                table: "METRICA_REQUISICAO",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);
        }
    }
}
