using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class BillFkUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HtBillings_HtBookingServices_BookingServiceId",
                table: "HtBillings");

            migrationBuilder.DropIndex(
                name: "IX_HtBillings_BookingServiceId",
                table: "HtBillings");

            migrationBuilder.DropColumn(
                name: "BookingServiceId",
                table: "HtBillings");

            migrationBuilder.CreateIndex(
                name: "IX_HtBillings_BookingId",
                table: "HtBillings",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_HtBillings_HtBookingServices_BookingId",
                table: "HtBillings",
                column: "BookingId",
                principalTable: "HtBookingServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HtBillings_HtBookingServices_BookingId",
                table: "HtBillings");

            migrationBuilder.DropIndex(
                name: "IX_HtBillings_BookingId",
                table: "HtBillings");

            migrationBuilder.AddColumn<long>(
                name: "BookingServiceId",
                table: "HtBillings",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HtBillings_BookingServiceId",
                table: "HtBillings",
                column: "BookingServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_HtBillings_HtBookingServices_BookingServiceId",
                table: "HtBillings",
                column: "BookingServiceId",
                principalTable: "HtBookingServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
