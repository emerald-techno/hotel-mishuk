using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class SalaryDtlAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PrSalaryDtlId",
                table: "AccTranMsts",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccTranMsts_PrSalaryDtlId",
                table: "AccTranMsts",
                column: "PrSalaryDtlId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccTranMsts_PrSalaryDtls_PrSalaryDtlId",
                table: "AccTranMsts",
                column: "PrSalaryDtlId",
                principalTable: "PrSalaryDtls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccTranMsts_PrSalaryDtls_PrSalaryDtlId",
                table: "AccTranMsts");

            migrationBuilder.DropIndex(
                name: "IX_AccTranMsts_PrSalaryDtlId",
                table: "AccTranMsts");

            migrationBuilder.DropColumn(
                name: "PrSalaryDtlId",
                table: "AccTranMsts");
        }
    }
}
