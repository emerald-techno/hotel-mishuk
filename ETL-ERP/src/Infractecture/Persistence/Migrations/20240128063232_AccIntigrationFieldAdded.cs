using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class AccIntigrationFieldAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PaymentId",
                table: "AccTranMsts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ServiceId",
                table: "AccLedgers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccTranMsts_PaymentId",
                table: "AccTranMsts",
                column: "PaymentId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_AccTranMsts_HtBookingPayments_PaymentId",
                table: "AccTranMsts",
                column: "PaymentId",
                principalTable: "HtBookingPayments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccLedgers_HtServices_ServiceId",
                table: "AccLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_AccTranMsts_HtBookingPayments_PaymentId",
                table: "AccTranMsts");

            migrationBuilder.DropIndex(
                name: "IX_AccTranMsts_PaymentId",
                table: "AccTranMsts");

            migrationBuilder.DropIndex(
                name: "IX_AccLedgers_ServiceId",
                table: "AccLedgers");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "AccTranMsts");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "AccLedgers");
        }
    }
}
