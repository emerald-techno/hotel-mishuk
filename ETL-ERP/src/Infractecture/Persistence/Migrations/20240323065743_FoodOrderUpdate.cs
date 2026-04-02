using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class FoodOrderUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CancelById",
                table: "RsFoodOrders",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelDate",
                table: "RsFoodOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodOrders_CancelById",
                table: "RsFoodOrders",
                column: "CancelById");

            migrationBuilder.AddForeignKey(
                name: "FK_RsFoodOrders_AspNetUsers_CancelById",
                table: "RsFoodOrders",
                column: "CancelById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RsFoodOrders_AspNetUsers_CancelById",
                table: "RsFoodOrders");

            migrationBuilder.DropIndex(
                name: "IX_RsFoodOrders_CancelById",
                table: "RsFoodOrders");

            migrationBuilder.DropColumn(
                name: "CancelById",
                table: "RsFoodOrders");

            migrationBuilder.DropColumn(
                name: "CancelDate",
                table: "RsFoodOrders");
        }
    }
}
