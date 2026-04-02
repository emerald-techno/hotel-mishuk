using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class BillIdAddedPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BillingId",
                table: "HtBookingPayments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingPayments_BillingId",
                table: "HtBookingPayments",
                column: "BillingId");

            migrationBuilder.AddForeignKey(
                name: "FK_HtBookingPayments_HtBillings_BillingId",
                table: "HtBookingPayments",
                column: "BillingId",
                principalTable: "HtBillings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HtBookingPayments_HtBillings_BillingId",
                table: "HtBookingPayments");

            migrationBuilder.DropIndex(
                name: "IX_HtBookingPayments_BillingId",
                table: "HtBookingPayments");

            migrationBuilder.DropColumn(
                name: "BillingId",
                table: "HtBookingPayments");
        }
    }
}
