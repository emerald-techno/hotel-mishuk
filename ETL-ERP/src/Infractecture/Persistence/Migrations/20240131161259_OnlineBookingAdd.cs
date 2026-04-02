using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class OnlineBookingAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OtherInfo",
                table: "HtRoomCategories",
                type: "nvarchar(600)",
                maxLength: 600,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OnlineBookingId",
                table: "HtBookingServices",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HtOnlineBookings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OnlineBookingNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    OnlineBookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArrivalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DepartureDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Adult = table.Column<double>(type: "float", nullable: false),
                    Child = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    GuestName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GuestMobile = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    GuestAddress = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    Rent = table.Column<double>(type: "float", nullable: false),
                    ServiceCharge = table.Column<double>(type: "float", nullable: false),
                    RoomCount = table.Column<int>(type: "int", nullable: false),
                    RoomCategoryId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtOnlineBookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtOnlineBookings_HtRoomCategories_RoomCategoryId",
                        column: x => x.RoomCategoryId,
                        principalTable: "HtRoomCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingServices_OnlineBookingId",
                table: "HtBookingServices",
                column: "OnlineBookingId");

            migrationBuilder.CreateIndex(
                name: "IX_HtOnlineBookings_RoomCategoryId",
                table: "HtOnlineBookings",
                column: "RoomCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_HtBookingServices_HtOnlineBookings_OnlineBookingId",
                table: "HtBookingServices",
                column: "OnlineBookingId",
                principalTable: "HtOnlineBookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HtBookingServices_HtOnlineBookings_OnlineBookingId",
                table: "HtBookingServices");

            migrationBuilder.DropTable(
                name: "HtOnlineBookings");

            migrationBuilder.DropIndex(
                name: "IX_HtBookingServices_OnlineBookingId",
                table: "HtBookingServices");

            migrationBuilder.DropColumn(
                name: "OnlineBookingId",
                table: "HtBookingServices");

            migrationBuilder.AlterColumn<string>(
                name: "OtherInfo",
                table: "HtRoomCategories",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(600)",
                oldMaxLength: 600,
                oldNullable: true);
        }
    }
}
