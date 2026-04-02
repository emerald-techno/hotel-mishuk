using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class InvBillPaymentUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "OrderMstId",
                table: "InventoryBillPayments",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "ReceiveId",
                table: "InventoryBillPayments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryBillPayments_ReceiveId",
                table: "InventoryBillPayments",
                column: "ReceiveId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryBillPayments_TranMsts_ReceiveId",
                table: "InventoryBillPayments",
                column: "ReceiveId",
                principalTable: "TranMsts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryBillPayments_TranMsts_ReceiveId",
                table: "InventoryBillPayments");

            migrationBuilder.DropIndex(
                name: "IX_InventoryBillPayments_ReceiveId",
                table: "InventoryBillPayments");

            migrationBuilder.DropColumn(
                name: "ReceiveId",
                table: "InventoryBillPayments");

            migrationBuilder.AlterColumn<long>(
                name: "OrderMstId",
                table: "InventoryBillPayments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
