using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hackathon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFieldsToSimulacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DT_CREATED_AT",
                table: "SIMULACAO",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TX_SOURCE_IP",
                table: "SIMULACAO",
                type: "TEXT",
                maxLength: 45,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TX_USER_AGENT",
                table: "SIMULACAO",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SIMULACAO_AUDITORIA_TEMPORAL",
                table: "SIMULACAO",
                columns: new[] { "DT_CREATED_AT", "DT_REFERENCIA" });

            migrationBuilder.CreateIndex(
                name: "IX_SIMULACAO_CREATED_AT",
                table: "SIMULACAO",
                column: "DT_CREATED_AT");

            migrationBuilder.CreateIndex(
                name: "IX_SIMULACAO_RELATORIO_VOLUME",
                table: "SIMULACAO",
                columns: new[] { "DT_REFERENCIA", "CO_PRODUTO", "PC_TAXA_JUROS" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SIMULACAO_AUDITORIA_TEMPORAL",
                table: "SIMULACAO");

            migrationBuilder.DropIndex(
                name: "IX_SIMULACAO_CREATED_AT",
                table: "SIMULACAO");

            migrationBuilder.DropIndex(
                name: "IX_SIMULACAO_RELATORIO_VOLUME",
                table: "SIMULACAO");

            migrationBuilder.DropColumn(
                name: "DT_CREATED_AT",
                table: "SIMULACAO");

            migrationBuilder.DropColumn(
                name: "TX_SOURCE_IP",
                table: "SIMULACAO");

            migrationBuilder.DropColumn(
                name: "TX_USER_AGENT",
                table: "SIMULACAO");
        }
    }
}
