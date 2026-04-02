using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class RoomMapCategoryIdAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RoomCategoryId",
                table: "HtRoomFacilityMaps",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomFacilityMaps_RoomCategoryId",
                table: "HtRoomFacilityMaps",
                column: "RoomCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_HtRoomFacilityMaps_HtRoomCategories_RoomCategoryId",
                table: "HtRoomFacilityMaps",
                column: "RoomCategoryId",
                principalTable: "HtRoomCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HtRoomFacilityMaps_HtRoomCategories_RoomCategoryId",
                table: "HtRoomFacilityMaps");

            migrationBuilder.DropIndex(
                name: "IX_HtRoomFacilityMaps_RoomCategoryId",
                table: "HtRoomFacilityMaps");

            migrationBuilder.DropColumn(
                name: "RoomCategoryId",
                table: "HtRoomFacilityMaps");
        }
    }
}
