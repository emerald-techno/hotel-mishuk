using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class DetailIdAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BillDtlId",
                table: "RsOrderPayments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RsOrderPayments_BillDtlId",
                table: "RsOrderPayments",
                column: "BillDtlId");

            migrationBuilder.AddForeignKey(
                name: "FK_RsOrderPayments_HtBillingDetails_BillDtlId",
                table: "RsOrderPayments",
                column: "BillDtlId",
                principalTable: "HtBillingDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RsOrderPayments_HtBillingDetails_BillDtlId",
                table: "RsOrderPayments");

            migrationBuilder.DropIndex(
                name: "IX_RsOrderPayments_BillDtlId",
                table: "RsOrderPayments");

            migrationBuilder.DropColumn(
                name: "BillDtlId",
                table: "RsOrderPayments");
        }
    }
}
