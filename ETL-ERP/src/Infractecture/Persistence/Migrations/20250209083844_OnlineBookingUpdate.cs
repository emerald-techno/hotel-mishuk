using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class OnlineBookingUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HtOnlineBookings_HtRoomCategories_RoomCategoryId",
                table: "HtOnlineBookings");

            migrationBuilder.DropIndex(
                name: "IX_HtOnlineBookings_RoomCategoryId",
                table: "HtOnlineBookings");

            migrationBuilder.DropColumn(
                name: "Adult",
                table: "HtOnlineBookings");

            migrationBuilder.DropColumn(
                name: "RoomCategoryId",
                table: "HtOnlineBookings");

            migrationBuilder.RenameColumn(
                name: "ServiceCharge",
                table: "HtOnlineBookings",
                newName: "TotalRent");

            migrationBuilder.RenameColumn(
                name: "RoomCount",
                table: "HtOnlineBookings",
                newName: "TotalRoomCount");

            migrationBuilder.RenameColumn(
                name: "Rent",
                table: "HtOnlineBookings",
                newName: "TotalChild");

            migrationBuilder.RenameColumn(
                name: "Child",
                table: "HtOnlineBookings",
                newName: "TotalAdult");

            migrationBuilder.AddColumn<string>(
                name: "GuestEmail",
                table: "HtOnlineBookings",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "HtOnlineBookings",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HtOnlineBookingDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckInDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOutDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RoomRent = table.Column<double>(type: "float", nullable: false),
                    RoomCount = table.Column<int>(type: "int", nullable: false),
                    TotalRent = table.Column<double>(type: "float", nullable: false),
                    Adult = table.Column<double>(type: "float", nullable: false),
                    Child = table.Column<double>(type: "float", nullable: false),
                    TotalDays = table.Column<double>(type: "float", nullable: false),
                    OnlineBookingId = table.Column<long>(type: "bigint", nullable: false),
                    RoomCategoryId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtOnlineBookingDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtOnlineBookingDetails_HtOnlineBookings_OnlineBookingId",
                        column: x => x.OnlineBookingId,
                        principalTable: "HtOnlineBookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtOnlineBookingDetails_HtRoomCategories_RoomCategoryId",
                        column: x => x.RoomCategoryId,
                        principalTable: "HtRoomCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HtOnlineBookingDetails_OnlineBookingId",
                table: "HtOnlineBookingDetails",
                column: "OnlineBookingId");

            migrationBuilder.CreateIndex(
                name: "IX_HtOnlineBookingDetails_RoomCategoryId",
                table: "HtOnlineBookingDetails",
                column: "RoomCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HtOnlineBookingDetails");

            migrationBuilder.DropColumn(
                name: "GuestEmail",
                table: "HtOnlineBookings");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "HtOnlineBookings");

            migrationBuilder.RenameColumn(
                name: "TotalRoomCount",
                table: "HtOnlineBookings",
                newName: "RoomCount");

            migrationBuilder.RenameColumn(
                name: "TotalRent",
                table: "HtOnlineBookings",
                newName: "ServiceCharge");

            migrationBuilder.RenameColumn(
                name: "TotalChild",
                table: "HtOnlineBookings",
                newName: "Rent");

            migrationBuilder.RenameColumn(
                name: "TotalAdult",
                table: "HtOnlineBookings",
                newName: "Child");

            migrationBuilder.AddColumn<double>(
                name: "Adult",
                table: "HtOnlineBookings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<long>(
                name: "RoomCategoryId",
                table: "HtOnlineBookings",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_HtOnlineBookings_RoomCategoryId",
                table: "HtOnlineBookings",
                column: "RoomCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_HtOnlineBookings_HtRoomCategories_RoomCategoryId",
                table: "HtOnlineBookings",
                column: "RoomCategoryId",
                principalTable: "HtRoomCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
