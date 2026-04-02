using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class BookingIdAddedFoodOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BookingId",
                table: "RsFoodOrders",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodOrders_BookingId",
                table: "RsFoodOrders",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_RsFoodOrders_HtBookingServices_BookingId",
                table: "RsFoodOrders",
                column: "BookingId",
                principalTable: "HtBookingServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RsFoodOrders_HtBookingServices_BookingId",
                table: "RsFoodOrders");

            migrationBuilder.DropIndex(
                name: "IX_RsFoodOrders_BookingId",
                table: "RsFoodOrders");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "RsFoodOrders");
        }
    }
}
