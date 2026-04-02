using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class CancelPropertyAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OOODate",
                table: "HtRoomInfos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OOORemakrs",
                table: "HtRoomInfos",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CancelBy",
                table: "HtBookingServices",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelDate",
                table: "HtBookingServices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                table: "HtBookingServices",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OOODate",
                table: "HtRoomInfos");

            migrationBuilder.DropColumn(
                name: "OOORemakrs",
                table: "HtRoomInfos");

            migrationBuilder.DropColumn(
                name: "CancelBy",
                table: "HtBookingServices");

            migrationBuilder.DropColumn(
                name: "CancelDate",
                table: "HtBookingServices");

            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "HtBookingServices");
        }
    }
}
