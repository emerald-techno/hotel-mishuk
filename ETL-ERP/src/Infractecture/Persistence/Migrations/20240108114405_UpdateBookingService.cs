using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class UpdateBookingService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HtBookingServices_HtRoomCategories_RoomCategoryId",
                table: "HtBookingServices");

            migrationBuilder.DropIndex(
                name: "IX_HtBookingServices_RoomCategoryId",
                table: "HtBookingServices");

            migrationBuilder.DropColumn(
                name: "RoomCategoryId",
                table: "HtBookingServices");

            migrationBuilder.AddColumn<string>(
                name: "BookingNo",
                table: "HtBookingServices",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "RoomCategoryId",
                table: "HtBookingRooms",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingRooms_RoomCategoryId",
                table: "HtBookingRooms",
                column: "RoomCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_HtBookingRooms_HtRoomCategories_RoomCategoryId",
                table: "HtBookingRooms",
                column: "RoomCategoryId",
                principalTable: "HtRoomCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HtBookingRooms_HtRoomCategories_RoomCategoryId",
                table: "HtBookingRooms");

            migrationBuilder.DropIndex(
                name: "IX_HtBookingRooms_RoomCategoryId",
                table: "HtBookingRooms");

            migrationBuilder.DropColumn(
                name: "BookingNo",
                table: "HtBookingServices");

            migrationBuilder.DropColumn(
                name: "RoomCategoryId",
                table: "HtBookingRooms");

            migrationBuilder.AddColumn<long>(
                name: "RoomCategoryId",
                table: "HtBookingServices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingServices_RoomCategoryId",
                table: "HtBookingServices",
                column: "RoomCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_HtBookingServices_HtRoomCategories_RoomCategoryId",
                table: "HtBookingServices",
                column: "RoomCategoryId",
                principalTable: "HtRoomCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
