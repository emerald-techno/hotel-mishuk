using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class RestaurantEntityAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RsCustomers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Salutation = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Dob = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PhotoUrl = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    GuestId = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RsCustomers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RsCustomers_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsCustomers_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsCustomers_HtGuestInfos_GuestId",
                        column: x => x.GuestId,
                        principalTable: "HtGuestInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RsCustomerTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RsCustomerTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RsCustomerTypes_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RsFoodCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ParentCategoryId = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RsFoodCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RsFoodCategories_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodCategories_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodCategories_RsFoodCategories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "RsFoodCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RsFoodTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RsFoodTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RsFoodTypes_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RsTables",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Capacity = table.Column<short>(type: "smallint", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RsTables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RsTables_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsTables_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RsFoodItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    VAT = table.Column<double>(type: "float", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    OfferRate = table.Column<double>(type: "float", nullable: false),
                    NetRate = table.Column<double>(type: "float", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RsFoodItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RsFoodItems_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodItems_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodItems_RsFoodCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "RsFoodCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RsFoodOrders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderStatus = table.Column<int>(type: "int", nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    OrderAmount = table.Column<double>(type: "float", nullable: false),
                    VAT = table.Column<double>(type: "float", nullable: false),
                    TAX = table.Column<double>(type: "float", nullable: false),
                    ServiceCharge = table.Column<double>(type: "float", nullable: false),
                    Discount = table.Column<double>(type: "float", nullable: false),
                    NetAmount = table.Column<double>(type: "float", nullable: false),
                    OrderDesc = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerTypeId = table.Column<long>(type: "bigint", nullable: false),
                    RoomId = table.Column<long>(type: "bigint", nullable: true),
                    TableId = table.Column<long>(type: "bigint", nullable: true),
                    WaiterId = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RsFoodOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RsFoodOrders_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodOrders_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodOrders_Employees_WaiterId",
                        column: x => x.WaiterId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodOrders_HtRoomInfos_RoomId",
                        column: x => x.RoomId,
                        principalTable: "HtRoomInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodOrders_RsCustomers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "RsCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodOrders_RsCustomerTypes_CustomerTypeId",
                        column: x => x.CustomerTypeId,
                        principalTable: "RsCustomerTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodOrders_RsTables_TableId",
                        column: x => x.TableId,
                        principalTable: "RsTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RsItemAvailabilities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Day = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    FoodId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RsItemAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RsItemAvailabilities_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsItemAvailabilities_RsFoodItems_FoodId",
                        column: x => x.FoodId,
                        principalTable: "RsFoodItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RsItemTypeMaps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    TypeId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RsItemTypeMaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RsItemTypeMaps_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsItemTypeMaps_RsFoodItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "RsFoodItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsItemTypeMaps_RsFoodTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "RsFoodTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RsFoodOrderItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    TotalAmount = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    FoodId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RsFoodOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RsFoodOrderItems_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodOrderItems_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodOrderItems_RsFoodItems_FoodId",
                        column: x => x.FoodId,
                        principalTable: "RsFoodItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsFoodOrderItems_RsFoodOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "RsFoodOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RsOrderPayments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaidAmount = table.Column<double>(type: "float", nullable: false),
                    PayMode = table.Column<int>(type: "int", nullable: false),
                    TransactionNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ChequeNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    AccountNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    BankId = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RsOrderPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RsOrderPayments_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsOrderPayments_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RsOrderPayments_RsFoodOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "RsFoodOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RsCustomers_ActionById",
                table: "RsCustomers",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_RsCustomers_GuestId",
                table: "RsCustomers",
                column: "GuestId");

            migrationBuilder.CreateIndex(
                name: "IX_RsCustomers_UpdatedById",
                table: "RsCustomers",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RsCustomerTypes_ActionById",
                table: "RsCustomerTypes",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodCategories_ActionById",
                table: "RsFoodCategories",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodCategories_ParentCategoryId",
                table: "RsFoodCategories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodCategories_UpdatedById",
                table: "RsFoodCategories",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodItems_ActionById",
                table: "RsFoodItems",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodItems_CategoryId",
                table: "RsFoodItems",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodItems_UpdatedById",
                table: "RsFoodItems",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodOrderItems_ActionById",
                table: "RsFoodOrderItems",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodOrderItems_FoodId",
                table: "RsFoodOrderItems",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodOrderItems_OrderId",
                table: "RsFoodOrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodOrderItems_UpdatedById",
                table: "RsFoodOrderItems",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodOrders_ActionById",
                table: "RsFoodOrders",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodOrders_CustomerId",
                table: "RsFoodOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodOrders_CustomerTypeId",
                table: "RsFoodOrders",
                column: "CustomerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodOrders_RoomId",
                table: "RsFoodOrders",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodOrders_TableId",
                table: "RsFoodOrders",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodOrders_UpdatedById",
                table: "RsFoodOrders",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodOrders_WaiterId",
                table: "RsFoodOrders",
                column: "WaiterId");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodTypes_ActionById",
                table: "RsFoodTypes",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_RsItemAvailabilities_ActionById",
                table: "RsItemAvailabilities",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_RsItemAvailabilities_FoodId",
                table: "RsItemAvailabilities",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_RsItemTypeMaps_ActionById",
                table: "RsItemTypeMaps",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_RsItemTypeMaps_ItemId",
                table: "RsItemTypeMaps",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RsItemTypeMaps_TypeId",
                table: "RsItemTypeMaps",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RsOrderPayments_ActionById",
                table: "RsOrderPayments",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_RsOrderPayments_OrderId",
                table: "RsOrderPayments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_RsOrderPayments_UpdatedById",
                table: "RsOrderPayments",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RsTables_ActionById",
                table: "RsTables",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_RsTables_UpdatedById",
                table: "RsTables",
                column: "UpdatedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RsFoodOrderItems");

            migrationBuilder.DropTable(
                name: "RsItemAvailabilities");

            migrationBuilder.DropTable(
                name: "RsItemTypeMaps");

            migrationBuilder.DropTable(
                name: "RsOrderPayments");

            migrationBuilder.DropTable(
                name: "RsFoodItems");

            migrationBuilder.DropTable(
                name: "RsFoodTypes");

            migrationBuilder.DropTable(
                name: "RsFoodOrders");

            migrationBuilder.DropTable(
                name: "RsFoodCategories");

            migrationBuilder.DropTable(
                name: "RsCustomers");

            migrationBuilder.DropTable(
                name: "RsCustomerTypes");

            migrationBuilder.DropTable(
                name: "RsTables");
        }
    }
}
