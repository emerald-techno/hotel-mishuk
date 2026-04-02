using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class InventoryAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryInfos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CategoryCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CategoryType = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    PhotoDocUrl = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryInfos_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CategoryInfos_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequsitionInfos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReqNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ReqDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsStatus = table.Column<short>(type: "smallint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReqById = table.Column<long>(type: "bigint", nullable: true),
                    DeptId = table.Column<long>(type: "bigint", nullable: true),
                    SubmitById = table.Column<long>(type: "bigint", nullable: false),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequsitionInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequsitionInfos_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequsitionInfos_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequsitionInfos_AspNetUsers_SubmitById",
                        column: x => x.SubmitById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequsitionInfos_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequsitionInfos_Departments_DeptId",
                        column: x => x.DeptId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequsitionInfos_Employees_ReqById",
                        column: x => x.ReqById,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierInfos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SupplierCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: true),
                    PhotoDoc = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierInfos_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierInfos_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnitInfos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    UnitCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitInfos_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnitInfos_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderMsts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderType = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    OrderMode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    ReceiveStatus = table.Column<short>(type: "smallint", nullable: false),
                    DeliveryDeadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompleteRemarks = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ReqId = table.Column<long>(type: "bigint", nullable: true),
                    SupplierId = table.Column<long>(type: "bigint", nullable: false),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderMsts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderMsts_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderMsts_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderMsts_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderMsts_RequsitionInfos_ReqId",
                        column: x => x.ReqId,
                        principalTable: "RequsitionInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderMsts_SupplierInfos_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "SupplierInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemInfos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ReorderQty = table.Column<double>(type: "float", nullable: false),
                    MaxQty = table.Column<double>(type: "float", nullable: false),
                    PhotoDocUrl = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    UnitId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemInfos_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemInfos_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemInfos_CategoryInfos_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "CategoryInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemInfos_UnitInfos_UnitId",
                        column: x => x.UnitId,
                        principalTable: "UnitInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TranMsts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TranNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TranDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TranType = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    TranFileUrl = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    QcDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QcDesc = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    QcFileUrl = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RefTranId = table.Column<long>(type: "bigint", nullable: true),
                    OrderId = table.Column<long>(type: "bigint", nullable: true),
                    SupplierId = table.Column<long>(type: "bigint", nullable: true),
                    TranById = table.Column<long>(type: "bigint", nullable: false),
                    IssueDeptId = table.Column<long>(type: "bigint", nullable: true),
                    IssueEmpId = table.Column<long>(type: "bigint", nullable: true),
                    QcById = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranMsts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TranMsts_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TranMsts_AspNetUsers_TranById",
                        column: x => x.TranById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TranMsts_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TranMsts_Departments_IssueDeptId",
                        column: x => x.IssueDeptId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TranMsts_Employees_IssueEmpId",
                        column: x => x.IssueEmpId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TranMsts_Employees_QcById",
                        column: x => x.QcById,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TranMsts_OrderMsts_OrderId",
                        column: x => x.OrderId,
                        principalTable: "OrderMsts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TranMsts_SupplierInfos_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "SupplierInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TranMsts_TranMsts_RefTranId",
                        column: x => x.RefTranId,
                        principalTable: "TranMsts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderDtls",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderQty = table.Column<double>(type: "float", nullable: false),
                    AprOrderQty = table.Column<double>(type: "float", nullable: false),
                    ActualAmount = table.Column<double>(type: "float", nullable: false),
                    Stock = table.Column<double>(type: "float", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    SlNo = table.Column<long>(type: "bigint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    LastOrderDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    ItemUnitId = table.Column<long>(type: "bigint", nullable: false),
                    LastOrderId = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDtls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDtls_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDtls_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDtls_ItemInfos_ItemId",
                        column: x => x.ItemId,
                        principalTable: "ItemInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDtls_OrderMsts_LastOrderId",
                        column: x => x.LastOrderId,
                        principalTable: "OrderMsts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDtls_OrderMsts_OrderId",
                        column: x => x.OrderId,
                        principalTable: "OrderMsts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDtls_UnitInfos_ItemUnitId",
                        column: x => x.ItemUnitId,
                        principalTable: "UnitInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequsitionInfoDtls",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReqQty = table.Column<double>(type: "float", nullable: false),
                    Stock = table.Column<double>(type: "float", nullable: false),
                    AprReqQty = table.Column<double>(type: "float", nullable: true),
                    IssueQty = table.Column<double>(type: "float", nullable: false),
                    SlNo = table.Column<long>(type: "bigint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ReqId = table.Column<long>(type: "bigint", nullable: false),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    ItemUnitId = table.Column<long>(type: "bigint", nullable: false),
                    LastReqId = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequsitionInfoDtls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequsitionInfoDtls_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequsitionInfoDtls_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequsitionInfoDtls_ItemInfos_ItemId",
                        column: x => x.ItemId,
                        principalTable: "ItemInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequsitionInfoDtls_RequsitionInfos_LastReqId",
                        column: x => x.LastReqId,
                        principalTable: "RequsitionInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequsitionInfoDtls_RequsitionInfos_ReqId",
                        column: x => x.ReqId,
                        principalTable: "RequsitionInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequsitionInfoDtls_UnitInfos_ItemUnitId",
                        column: x => x.ItemUnitId,
                        principalTable: "UnitInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NoteInfos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoteDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoteDesc = table.Column<DateTime>(type: "datetime2", maxLength: 350, nullable: false),
                    SlNo = table.Column<long>(type: "bigint", nullable: false),
                    NoteFileUrl = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoteById = table.Column<long>(type: "bigint", nullable: false),
                    TranMstId = table.Column<long>(type: "bigint", nullable: true),
                    OrderId = table.Column<long>(type: "bigint", nullable: true),
                    ReqMstId = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NoteInfos_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NoteInfos_AspNetUsers_NoteById",
                        column: x => x.NoteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NoteInfos_OrderMsts_OrderId",
                        column: x => x.OrderId,
                        principalTable: "OrderMsts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NoteInfos_RequsitionInfos_ReqMstId",
                        column: x => x.ReqMstId,
                        principalTable: "RequsitionInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NoteInfos_TranMsts_TranMstId",
                        column: x => x.TranMstId,
                        principalTable: "TranMsts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TranDtls",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SlNo = table.Column<long>(type: "bigint", nullable: false),
                    ItemQty = table.Column<double>(type: "float", nullable: false),
                    Stock = table.Column<double>(type: "float", nullable: false),
                    OrderQty = table.Column<double>(type: "float", nullable: false),
                    IsQcPass = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TranMstId = table.Column<long>(type: "bigint", nullable: false),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    ItemUnitId = table.Column<long>(type: "bigint", nullable: false),
                    OrderDtlId = table.Column<long>(type: "bigint", nullable: true),
                    ReqDtlId = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranDtls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TranDtls_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TranDtls_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TranDtls_ItemInfos_ItemId",
                        column: x => x.ItemId,
                        principalTable: "ItemInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TranDtls_OrderDtls_OrderDtlId",
                        column: x => x.OrderDtlId,
                        principalTable: "OrderDtls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TranDtls_RequsitionInfoDtls_ReqDtlId",
                        column: x => x.ReqDtlId,
                        principalTable: "RequsitionInfoDtls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TranDtls_TranMsts_TranMstId",
                        column: x => x.TranMstId,
                        principalTable: "TranMsts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TranDtls_UnitInfos_ItemUnitId",
                        column: x => x.ItemUnitId,
                        principalTable: "UnitInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryInfos_ActionById",
                table: "CategoryInfos",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryInfos_UpdatedById",
                table: "CategoryInfos",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ItemInfos_ActionById",
                table: "ItemInfos",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_ItemInfos_CategoryId",
                table: "ItemInfos",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemInfos_UnitId",
                table: "ItemInfos",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemInfos_UpdatedById",
                table: "ItemInfos",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_NoteInfos_ActionById",
                table: "NoteInfos",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_NoteInfos_NoteById",
                table: "NoteInfos",
                column: "NoteById");

            migrationBuilder.CreateIndex(
                name: "IX_NoteInfos_OrderId",
                table: "NoteInfos",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_NoteInfos_ReqMstId",
                table: "NoteInfos",
                column: "ReqMstId");

            migrationBuilder.CreateIndex(
                name: "IX_NoteInfos_TranMstId",
                table: "NoteInfos",
                column: "TranMstId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDtls_ActionById",
                table: "OrderDtls",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDtls_ItemId",
                table: "OrderDtls",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDtls_ItemUnitId",
                table: "OrderDtls",
                column: "ItemUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDtls_LastOrderId",
                table: "OrderDtls",
                column: "LastOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDtls_OrderId",
                table: "OrderDtls",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDtls_UpdatedById",
                table: "OrderDtls",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OrderMsts_ActionById",
                table: "OrderMsts",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_OrderMsts_ApprovedById",
                table: "OrderMsts",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_OrderMsts_ReqId",
                table: "OrderMsts",
                column: "ReqId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderMsts_SupplierId",
                table: "OrderMsts",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderMsts_UpdatedById",
                table: "OrderMsts",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RequsitionInfoDtls_ActionById",
                table: "RequsitionInfoDtls",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_RequsitionInfoDtls_ItemId",
                table: "RequsitionInfoDtls",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RequsitionInfoDtls_ItemUnitId",
                table: "RequsitionInfoDtls",
                column: "ItemUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_RequsitionInfoDtls_LastReqId",
                table: "RequsitionInfoDtls",
                column: "LastReqId");

            migrationBuilder.CreateIndex(
                name: "IX_RequsitionInfoDtls_ReqId",
                table: "RequsitionInfoDtls",
                column: "ReqId");

            migrationBuilder.CreateIndex(
                name: "IX_RequsitionInfoDtls_UpdatedById",
                table: "RequsitionInfoDtls",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RequsitionInfos_ActionById",
                table: "RequsitionInfos",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_RequsitionInfos_ApprovedById",
                table: "RequsitionInfos",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_RequsitionInfos_DeptId",
                table: "RequsitionInfos",
                column: "DeptId");

            migrationBuilder.CreateIndex(
                name: "IX_RequsitionInfos_ReqById",
                table: "RequsitionInfos",
                column: "ReqById");

            migrationBuilder.CreateIndex(
                name: "IX_RequsitionInfos_SubmitById",
                table: "RequsitionInfos",
                column: "SubmitById");

            migrationBuilder.CreateIndex(
                name: "IX_RequsitionInfos_UpdatedById",
                table: "RequsitionInfos",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierInfos_ActionById",
                table: "SupplierInfos",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierInfos_UpdatedById",
                table: "SupplierInfos",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TranDtls_ActionById",
                table: "TranDtls",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_TranDtls_ItemId",
                table: "TranDtls",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TranDtls_ItemUnitId",
                table: "TranDtls",
                column: "ItemUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_TranDtls_OrderDtlId",
                table: "TranDtls",
                column: "OrderDtlId");

            migrationBuilder.CreateIndex(
                name: "IX_TranDtls_ReqDtlId",
                table: "TranDtls",
                column: "ReqDtlId");

            migrationBuilder.CreateIndex(
                name: "IX_TranDtls_TranMstId",
                table: "TranDtls",
                column: "TranMstId");

            migrationBuilder.CreateIndex(
                name: "IX_TranDtls_UpdatedById",
                table: "TranDtls",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TranMsts_ActionById",
                table: "TranMsts",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_TranMsts_IssueDeptId",
                table: "TranMsts",
                column: "IssueDeptId");

            migrationBuilder.CreateIndex(
                name: "IX_TranMsts_IssueEmpId",
                table: "TranMsts",
                column: "IssueEmpId");

            migrationBuilder.CreateIndex(
                name: "IX_TranMsts_OrderId",
                table: "TranMsts",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_TranMsts_QcById",
                table: "TranMsts",
                column: "QcById");

            migrationBuilder.CreateIndex(
                name: "IX_TranMsts_RefTranId",
                table: "TranMsts",
                column: "RefTranId");

            migrationBuilder.CreateIndex(
                name: "IX_TranMsts_SupplierId",
                table: "TranMsts",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_TranMsts_TranById",
                table: "TranMsts",
                column: "TranById");

            migrationBuilder.CreateIndex(
                name: "IX_TranMsts_UpdatedById",
                table: "TranMsts",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UnitInfos_ActionById",
                table: "UnitInfos",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_UnitInfos_UpdatedById",
                table: "UnitInfos",
                column: "UpdatedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NoteInfos");

            migrationBuilder.DropTable(
                name: "TranDtls");

            migrationBuilder.DropTable(
                name: "OrderDtls");

            migrationBuilder.DropTable(
                name: "RequsitionInfoDtls");

            migrationBuilder.DropTable(
                name: "TranMsts");

            migrationBuilder.DropTable(
                name: "ItemInfos");

            migrationBuilder.DropTable(
                name: "OrderMsts");

            migrationBuilder.DropTable(
                name: "CategoryInfos");

            migrationBuilder.DropTable(
                name: "UnitInfos");

            migrationBuilder.DropTable(
                name: "RequsitionInfos");

            migrationBuilder.DropTable(
                name: "SupplierInfos");
        }
    }
}
