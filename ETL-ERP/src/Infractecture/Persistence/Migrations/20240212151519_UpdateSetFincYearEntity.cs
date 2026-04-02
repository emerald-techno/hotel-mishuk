using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class UpdateSetFincYearEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ActionById",
                table: "SetFincYears",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActionDate",
                table: "SetFincYears",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "SetFincYears",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedById",
                table: "SetFincYears",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SetFincYears_ActionById",
                table: "SetFincYears",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_SetFincYears_UpdatedById",
                table: "SetFincYears",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_SetFincYears_AspNetUsers_ActionById",
                table: "SetFincYears",
                column: "ActionById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SetFincYears_AspNetUsers_UpdatedById",
                table: "SetFincYears",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SetFincYears_AspNetUsers_ActionById",
                table: "SetFincYears");

            migrationBuilder.DropForeignKey(
                name: "FK_SetFincYears_AspNetUsers_UpdatedById",
                table: "SetFincYears");

            migrationBuilder.DropIndex(
                name: "IX_SetFincYears_ActionById",
                table: "SetFincYears");

            migrationBuilder.DropIndex(
                name: "IX_SetFincYears_UpdatedById",
                table: "SetFincYears");

            migrationBuilder.DropColumn(
                name: "ActionById",
                table: "SetFincYears");

            migrationBuilder.DropColumn(
                name: "ActionDate",
                table: "SetFincYears");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "SetFincYears");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "SetFincYears");
        }
    }
}
