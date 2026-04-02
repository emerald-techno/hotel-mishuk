using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class UpdateTaskAssign : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HkTaskAssigns_HkTaskAssigns_AssignId",
                table: "HkTaskAssigns");

            migrationBuilder.AddForeignKey(
                name: "FK_HkTaskAssigns_HkRoomAssigns_AssignId",
                table: "HkTaskAssigns",
                column: "AssignId",
                principalTable: "HkRoomAssigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HkTaskAssigns_HkRoomAssigns_AssignId",
                table: "HkTaskAssigns");

            migrationBuilder.AddForeignKey(
                name: "FK_HkTaskAssigns_HkTaskAssigns_AssignId",
                table: "HkTaskAssigns",
                column: "AssignId",
                principalTable: "HkTaskAssigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
