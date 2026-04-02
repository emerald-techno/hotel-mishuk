using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class FoodOrderWaiterUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RsFoodOrders_Employees_WaiterId",
                table: "RsFoodOrders");

            migrationBuilder.AddForeignKey(
                name: "FK_RsFoodOrders_RsWaiters_WaiterId",
                table: "RsFoodOrders",
                column: "WaiterId",
                principalTable: "RsWaiters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RsFoodOrders_RsWaiters_WaiterId",
                table: "RsFoodOrders");

            migrationBuilder.AddForeignKey(
                name: "FK_RsFoodOrders_Employees_WaiterId",
                table: "RsFoodOrders",
                column: "WaiterId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
