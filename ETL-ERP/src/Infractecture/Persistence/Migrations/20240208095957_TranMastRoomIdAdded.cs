using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class TranMastRoomIdAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "IssueRoomId",
                table: "TranMsts",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TranMsts_IssueRoomId",
                table: "TranMsts",
                column: "IssueRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_TranMsts_HtRoomInfos_IssueRoomId",
                table: "TranMsts",
                column: "IssueRoomId",
                principalTable: "HtRoomInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TranMsts_HtRoomInfos_IssueRoomId",
                table: "TranMsts");

            migrationBuilder.DropIndex(
                name: "IX_TranMsts_IssueRoomId",
                table: "TranMsts");

            migrationBuilder.DropColumn(
                name: "IssueRoomId",
                table: "TranMsts");
        }
    }
}
