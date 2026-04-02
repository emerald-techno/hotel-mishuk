using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class RoomStatusHstAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HtRoomStatusHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomStatus = table.Column<int>(type: "int", nullable: false),
                    StatusDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RoomId = table.Column<long>(type: "bigint", nullable: false),
                    RoomStatusById = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtRoomStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtRoomStatusHistories_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtRoomStatusHistories_AspNetUsers_RoomStatusById",
                        column: x => x.RoomStatusById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtRoomStatusHistories_HtRoomInfos_RoomId",
                        column: x => x.RoomId,
                        principalTable: "HtRoomInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomStatusHistories_ActionById",
                table: "HtRoomStatusHistories",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomStatusHistories_RoomId",
                table: "HtRoomStatusHistories",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomStatusHistories_RoomStatusById",
                table: "HtRoomStatusHistories",
                column: "RoomStatusById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HtRoomStatusHistories");
        }
    }
}
