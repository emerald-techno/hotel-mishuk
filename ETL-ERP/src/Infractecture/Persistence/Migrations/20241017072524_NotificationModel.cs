using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class NotificationModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NtfEventInfos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EventCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    NotifyBefore = table.Column<short>(type: "smallint", nullable: false),
                    IsEmailNtf = table.Column<bool>(type: "bit", nullable: false),
                    IsSmsNtf = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NtfEventInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NtfEventInfos_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NtfEventInfos_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NtfNotificationMsgs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NtfMsg = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    NtfTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSeen = table.Column<bool>(type: "bit", nullable: false),
                    SeenTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NtfNotificationMsgs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NtfNotificationMsgs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NtfNotificationMsgs_NtfEventInfos_EventId",
                        column: x => x.EventId,
                        principalTable: "NtfEventInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NtfUserSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SettingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsEnable = table.Column<bool>(type: "bit", nullable: false),
                    IsEmail = table.Column<bool>(type: "bit", nullable: false),
                    IsSms = table.Column<bool>(type: "bit", nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NtfUserSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NtfUserSettings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NtfUserSettings_NtfEventInfos_EventId",
                        column: x => x.EventId,
                        principalTable: "NtfEventInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NtfEventInfos_ActionById",
                table: "NtfEventInfos",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_NtfEventInfos_UpdatedById",
                table: "NtfEventInfos",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_NtfNotificationMsgs_EventId",
                table: "NtfNotificationMsgs",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_NtfNotificationMsgs_UserId",
                table: "NtfNotificationMsgs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NtfUserSettings_EventId",
                table: "NtfUserSettings",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_NtfUserSettings_UserId",
                table: "NtfUserSettings",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NtfNotificationMsgs");

            migrationBuilder.DropTable(
                name: "NtfUserSettings");

            migrationBuilder.DropTable(
                name: "NtfEventInfos");
        }
    }
}
