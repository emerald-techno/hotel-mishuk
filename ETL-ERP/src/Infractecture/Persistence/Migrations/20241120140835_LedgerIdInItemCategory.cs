using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class LedgerIdInItemCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LedgerId",
                table: "CategoryInfos",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoryInfos_LedgerId",
                table: "CategoryInfos",
                column: "LedgerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryInfos_AccLedgers_LedgerId",
                table: "CategoryInfos",
                column: "LedgerId",
                principalTable: "AccLedgers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryInfos_AccLedgers_LedgerId",
                table: "CategoryInfos");

            migrationBuilder.DropIndex(
                name: "IX_CategoryInfos_LedgerId",
                table: "CategoryInfos");

            migrationBuilder.DropColumn(
                name: "LedgerId",
                table: "CategoryInfos");
        }
    }
}
