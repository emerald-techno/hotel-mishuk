using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class Ingredient_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RsFoodIngredients_RsFoodOrders_FoodItemId",
                table: "RsFoodIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_RsFoodIngredientsHsts_RsFoodOrderItems_FoodItemId",
                table: "RsFoodIngredientsHsts");

            migrationBuilder.AddForeignKey(
                name: "FK_RsFoodIngredients_RsFoodItems_FoodItemId",
                table: "RsFoodIngredients",
                column: "FoodItemId",
                principalTable: "RsFoodItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RsFoodIngredientsHsts_RsFoodItems_FoodItemId",
                table: "RsFoodIngredientsHsts",
                column: "FoodItemId",
                principalTable: "RsFoodItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RsFoodIngredients_RsFoodItems_FoodItemId",
                table: "RsFoodIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_RsFoodIngredientsHsts_RsFoodItems_FoodItemId",
                table: "RsFoodIngredientsHsts");

            migrationBuilder.AddForeignKey(
                name: "FK_RsFoodIngredients_RsFoodOrders_FoodItemId",
                table: "RsFoodIngredients",
                column: "FoodItemId",
                principalTable: "RsFoodOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RsFoodIngredientsHsts_RsFoodOrderItems_FoodItemId",
                table: "RsFoodIngredientsHsts",
                column: "FoodItemId",
                principalTable: "RsFoodOrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
