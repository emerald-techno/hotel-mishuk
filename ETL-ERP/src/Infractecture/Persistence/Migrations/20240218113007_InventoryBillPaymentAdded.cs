using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class InventoryBillPaymentAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryBillPayments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BillDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BillAmount = table.Column<double>(type: "float", nullable: false),
                    PayMode = table.Column<int>(type: "int", nullable: false),
                    BillFileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAdvance = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    OrderMstId = table.Column<long>(type: "bigint", nullable: false),
                    BillById = table.Column<long>(type: "bigint", nullable: false),
                    SupplierId = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryBillPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryBillPayments_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryBillPayments_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryBillPayments_AspNetUsers_BillById",
                        column: x => x.BillById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryBillPayments_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryBillPayments_OrderMsts_OrderMstId",
                        column: x => x.OrderMstId,
                        principalTable: "OrderMsts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryBillPayments_SupplierInfos_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "SupplierInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryBillPayments_ActionById",
                table: "InventoryBillPayments",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryBillPayments_ApprovedById",
                table: "InventoryBillPayments",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryBillPayments_BillById",
                table: "InventoryBillPayments",
                column: "BillById");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryBillPayments_OrderMstId",
                table: "InventoryBillPayments",
                column: "OrderMstId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryBillPayments_SupplierId",
                table: "InventoryBillPayments",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryBillPayments_UpdatedById",
                table: "InventoryBillPayments",
                column: "UpdatedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryBillPayments");
        }
    }
}
