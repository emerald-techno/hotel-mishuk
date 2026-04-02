using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class DailySummaryEntityUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BookingRoomId",
                table: "RsFoodOrders",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FNBReceivable",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FrontOfficeReceivable",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodOrders_BookingRoomId",
                table: "RsFoodOrders",
                column: "BookingRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_RsFoodOrders_HtBookingRooms_BookingRoomId",
                table: "RsFoodOrders",
                column: "BookingRoomId",
                principalTable: "HtBookingRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RsFoodOrders_HtBookingRooms_BookingRoomId",
                table: "RsFoodOrders");

            migrationBuilder.DropIndex(
                name: "IX_RsFoodOrders_BookingRoomId",
                table: "RsFoodOrders");

            migrationBuilder.DropColumn(
                name: "BookingRoomId",
                table: "RsFoodOrders");

            migrationBuilder.DropColumn(
                name: "FNBReceivable",
                table: "HtDailyAuditSummaries");

            migrationBuilder.DropColumn(
                name: "FrontOfficeReceivable",
                table: "HtDailyAuditSummaries");
        }
    }
}
