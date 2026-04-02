using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class VoucherRefKotAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LedgerId",
                table: "TranMsts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KotNo",
                table: "RsFoodOrderItems",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "InvPaymentId",
                table: "AccTranMsts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RsPaymentId",
                table: "AccTranMsts",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccTranMsts_InvPaymentId",
                table: "AccTranMsts",
                column: "InvPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_AccTranMsts_RsPaymentId",
                table: "AccTranMsts",
                column: "RsPaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccTranMsts_InventoryBillPayments_InvPaymentId",
                table: "AccTranMsts",
                column: "InvPaymentId",
                principalTable: "InventoryBillPayments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccTranMsts_RsOrderPayments_RsPaymentId",
                table: "AccTranMsts",
                column: "RsPaymentId",
                principalTable: "RsOrderPayments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccTranMsts_InventoryBillPayments_InvPaymentId",
                table: "AccTranMsts");

            migrationBuilder.DropForeignKey(
                name: "FK_AccTranMsts_RsOrderPayments_RsPaymentId",
                table: "AccTranMsts");

            migrationBuilder.DropIndex(
                name: "IX_AccTranMsts_InvPaymentId",
                table: "AccTranMsts");

            migrationBuilder.DropIndex(
                name: "IX_AccTranMsts_RsPaymentId",
                table: "AccTranMsts");

            migrationBuilder.DropColumn(
                name: "LedgerId",
                table: "TranMsts");

            migrationBuilder.DropColumn(
                name: "KotNo",
                table: "RsFoodOrderItems");

            migrationBuilder.DropColumn(
                name: "InvPaymentId",
                table: "AccTranMsts");

            migrationBuilder.DropColumn(
                name: "RsPaymentId",
                table: "AccTranMsts");
        }
    }
}
