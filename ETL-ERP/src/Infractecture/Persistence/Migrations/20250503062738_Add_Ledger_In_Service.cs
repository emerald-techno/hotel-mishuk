using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class Add_Ledger_In_Service : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccLedgers_HtServices_ServiceId",
                table: "AccLedgers");

            migrationBuilder.DropIndex(
                name: "IX_AccLedgers_ServiceId",
                table: "AccLedgers");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "AccLedgers");

            migrationBuilder.AddColumn<long>(
                name: "LedgerId",
                table: "HtServices",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HtServices_LedgerId",
                table: "HtServices",
                column: "LedgerId");

            migrationBuilder.AddForeignKey(
                name: "FK_HtServices_AccLedgers_LedgerId",
                table: "HtServices",
                column: "LedgerId",
                principalTable: "AccLedgers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HtServices_AccLedgers_LedgerId",
                table: "HtServices");

            migrationBuilder.DropIndex(
                name: "IX_HtServices_LedgerId",
                table: "HtServices");

            migrationBuilder.DropColumn(
                name: "LedgerId",
                table: "HtServices");

            migrationBuilder.AddColumn<long>(
                name: "ServiceId",
                table: "AccLedgers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccLedgers_ServiceId",
                table: "AccLedgers",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccLedgers_HtServices_ServiceId",
                table: "AccLedgers",
                column: "ServiceId",
                principalTable: "HtServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
