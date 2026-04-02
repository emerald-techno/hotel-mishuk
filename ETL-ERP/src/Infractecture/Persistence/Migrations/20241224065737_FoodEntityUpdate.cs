using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class FoodEntityUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsServed",
                table: "RsFoodOrderItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ServedById",
                table: "RsFoodOrderItems",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ServedTime",
                table: "RsFoodOrderItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSetMenuItem",
                table: "RsFoodItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ItemConvertions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    ConvertedQuantity = table.Column<double>(type: "float", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisableDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    UnitId = table.Column<long>(type: "bigint", nullable: false),
                    ConvertedUnitId = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemConvertions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemConvertions_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemConvertions_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemConvertions_ItemInfos_ItemId",
                        column: x => x.ItemId,
                        principalTable: "ItemInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemConvertions_UnitInfos_ConvertedUnitId",
                        column: x => x.ConvertedUnitId,
                        principalTable: "UnitInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemConvertions_UnitInfos_UnitId",
                        column: x => x.UnitId,
                        principalTable: "UnitInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RsFoodIngredients",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisableDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    FoodItemId = table.Column<long>(type: "bigint", nullable: false),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    UnitId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RsFoodIngredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RsFoodIngredients_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodIngredients_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodIngredients_ItemInfos_ItemId",
                        column: x => x.ItemId,
                        principalTable: "ItemInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodIngredients_RsFoodOrders_FoodItemId",
                        column: x => x.FoodItemId,
                        principalTable: "RsFoodOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodIngredients_UnitInfos_UnitId",
                        column: x => x.UnitId,
                        principalTable: "UnitInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RsFoodIngredientsHsts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    SysPrice = table.Column<double>(type: "float", nullable: false),
                    SysAmount = table.Column<double>(type: "float", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisableDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    OrderItemId = table.Column<long>(type: "bigint", nullable: false),
                    FoodItemId = table.Column<long>(type: "bigint", nullable: false),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    UnitId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RsFoodIngredientsHsts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RsFoodIngredientsHsts_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodIngredientsHsts_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodIngredientsHsts_ItemInfos_ItemId",
                        column: x => x.ItemId,
                        principalTable: "ItemInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodIngredientsHsts_RsFoodOrderItems_FoodItemId",
                        column: x => x.FoodItemId,
                        principalTable: "RsFoodOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodIngredientsHsts_RsFoodOrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "RsFoodOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodIngredientsHsts_UnitInfos_UnitId",
                        column: x => x.UnitId,
                        principalTable: "UnitInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RsFoodSetItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsEnable = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    FoodItemId = table.Column<long>(type: "bigint", nullable: false),
                    SetFoodItemId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RsFoodSetItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RsFoodSetItems_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodSetItems_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodSetItems_RsFoodItems_FoodItemId",
                        column: x => x.FoodItemId,
                        principalTable: "RsFoodItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodSetItems_RsFoodItems_SetFoodItemId",
                        column: x => x.SetFoodItemId,
                        principalTable: "RsFoodItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodOrderItems_ServedById",
                table: "RsFoodOrderItems",
                column: "ServedById");

            migrationBuilder.CreateIndex(
                name: "IX_ItemConvertions_ActionById",
                table: "ItemConvertions",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_ItemConvertions_ConvertedUnitId",
                table: "ItemConvertions",
                column: "ConvertedUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemConvertions_ItemId",
                table: "ItemConvertions",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemConvertions_UnitId",
                table: "ItemConvertions",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemConvertions_UpdatedById",
                table: "ItemConvertions",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodIngredients_ActionById",
                table: "RsFoodIngredients",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodIngredients_FoodItemId",
                table: "RsFoodIngredients",
                column: "FoodItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodIngredients_ItemId",
                table: "RsFoodIngredients",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodIngredients_UnitId",
                table: "RsFoodIngredients",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodIngredients_UpdatedById",
                table: "RsFoodIngredients",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodIngredientsHsts_ActionById",
                table: "RsFoodIngredientsHsts",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodIngredientsHsts_FoodItemId",
                table: "RsFoodIngredientsHsts",
                column: "FoodItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodIngredientsHsts_ItemId",
                table: "RsFoodIngredientsHsts",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodIngredientsHsts_OrderItemId",
                table: "RsFoodIngredientsHsts",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodIngredientsHsts_UnitId",
                table: "RsFoodIngredientsHsts",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodIngredientsHsts_UpdatedById",
                table: "RsFoodIngredientsHsts",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodSetItems_ActionById",
                table: "RsFoodSetItems",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodSetItems_FoodItemId",
                table: "RsFoodSetItems",
                column: "FoodItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodSetItems_SetFoodItemId",
                table: "RsFoodSetItems",
                column: "SetFoodItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodSetItems_UpdatedById",
                table: "RsFoodSetItems",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_RsFoodOrderItems_AspNetUsers_ServedById",
                table: "RsFoodOrderItems",
                column: "ServedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RsFoodOrderItems_AspNetUsers_ServedById",
                table: "RsFoodOrderItems");

            migrationBuilder.DropTable(
                name: "ItemConvertions");

            migrationBuilder.DropTable(
                name: "RsFoodIngredients");

            migrationBuilder.DropTable(
                name: "RsFoodIngredientsHsts");

            migrationBuilder.DropTable(
                name: "RsFoodSetItems");

            migrationBuilder.DropIndex(
                name: "IX_RsFoodOrderItems_ServedById",
                table: "RsFoodOrderItems");

            migrationBuilder.DropColumn(
                name: "IsServed",
                table: "RsFoodOrderItems");

            migrationBuilder.DropColumn(
                name: "ServedById",
                table: "RsFoodOrderItems");

            migrationBuilder.DropColumn(
                name: "ServedTime",
                table: "RsFoodOrderItems");

            migrationBuilder.DropColumn(
                name: "IsSetMenuItem",
                table: "RsFoodItems");
        }
    }
}
