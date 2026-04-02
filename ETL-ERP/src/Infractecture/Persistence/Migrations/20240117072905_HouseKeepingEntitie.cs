using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class HouseKeepingEntitie : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HkRoomAssigns",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    HouseKeeperId = table.Column<long>(type: "bigint", nullable: false),
                    RoomId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HkRoomAssigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HkRoomAssigns_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HkRoomAssigns_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HkRoomAssigns_Employees_HouseKeeperId",
                        column: x => x.HouseKeeperId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HkRoomAssigns_HtRoomInfos_RoomId",
                        column: x => x.RoomId,
                        principalTable: "HtRoomInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HkTaskTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HkTaskTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HkTaskTypes_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HkTaskTypes_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HtBillings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    BillDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmmount = table.Column<double>(type: "float", nullable: false),
                    Vat = table.Column<double>(type: "float", nullable: false),
                    Tax = table.Column<double>(type: "float", nullable: false),
                    Discount = table.Column<double>(type: "float", nullable: false),
                    NetAmount = table.Column<double>(type: "float", nullable: false),
                    BillStatus = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    BookingId = table.Column<long>(type: "bigint", nullable: false),
                    BookingServiceId = table.Column<long>(type: "bigint", nullable: true),
                    BillById = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtBillings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtBillings_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtBillings_AspNetUsers_BillById",
                        column: x => x.BillById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtBillings_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtBillings_HtBookingServices_BookingServiceId",
                        column: x => x.BookingServiceId,
                        principalTable: "HtBookingServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HtServices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ServiceCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtServices_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtServices_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HkTaskNames",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TypeId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HkTaskNames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HkTaskNames_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HkTaskNames_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HkTaskNames_HkTaskTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "HkTaskTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HtBillingDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    VAT = table.Column<double>(type: "float", nullable: false),
                    Tax = table.Column<double>(type: "float", nullable: false),
                    Discount = table.Column<double>(type: "float", nullable: false),
                    NetAmount = table.Column<double>(type: "float", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    BillId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    BookingRoomId = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtBillingDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtBillingDetails_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtBillingDetails_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtBillingDetails_HtBillings_BillId",
                        column: x => x.BillId,
                        principalTable: "HtBillings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtBillingDetails_HtBookingRooms_BookingRoomId",
                        column: x => x.BookingRoomId,
                        principalTable: "HtBookingRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtBillingDetails_HtServices_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "HtServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HkTaskAssigns",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignRemarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    CompleteRemarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    AuditorRemarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    AuditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    AssignId = table.Column<long>(type: "bigint", nullable: false),
                    TaskId = table.Column<long>(type: "bigint", nullable: false),
                    AuditorId = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HkTaskAssigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HkTaskAssigns_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HkTaskAssigns_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HkTaskAssigns_Employees_AuditorId",
                        column: x => x.AuditorId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HkTaskAssigns_HkTaskAssigns_AssignId",
                        column: x => x.AssignId,
                        principalTable: "HkTaskAssigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HkTaskAssigns_HkTaskNames_TaskId",
                        column: x => x.TaskId,
                        principalTable: "HkTaskNames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HkTaskRates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    TaskId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HkTaskRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HkTaskRates_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HkTaskRates_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HkTaskRates_HkTaskNames_TaskId",
                        column: x => x.TaskId,
                        principalTable: "HkTaskNames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HkTaskRates_ItemInfos_ItemId",
                        column: x => x.ItemId,
                        principalTable: "ItemInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HkRoomAssigns_ActionById",
                table: "HkRoomAssigns",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HkRoomAssigns_HouseKeeperId",
                table: "HkRoomAssigns",
                column: "HouseKeeperId");

            migrationBuilder.CreateIndex(
                name: "IX_HkRoomAssigns_RoomId",
                table: "HkRoomAssigns",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_HkRoomAssigns_UpdatedById",
                table: "HkRoomAssigns",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HkTaskAssigns_ActionById",
                table: "HkTaskAssigns",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HkTaskAssigns_AssignId",
                table: "HkTaskAssigns",
                column: "AssignId");

            migrationBuilder.CreateIndex(
                name: "IX_HkTaskAssigns_AuditorId",
                table: "HkTaskAssigns",
                column: "AuditorId");

            migrationBuilder.CreateIndex(
                name: "IX_HkTaskAssigns_TaskId",
                table: "HkTaskAssigns",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_HkTaskAssigns_UpdatedById",
                table: "HkTaskAssigns",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HkTaskNames_ActionById",
                table: "HkTaskNames",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HkTaskNames_TypeId",
                table: "HkTaskNames",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HkTaskNames_UpdatedById",
                table: "HkTaskNames",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HkTaskRates_ActionById",
                table: "HkTaskRates",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HkTaskRates_ItemId",
                table: "HkTaskRates",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_HkTaskRates_TaskId",
                table: "HkTaskRates",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_HkTaskRates_UpdatedById",
                table: "HkTaskRates",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HkTaskTypes_ActionById",
                table: "HkTaskTypes",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HkTaskTypes_UpdatedById",
                table: "HkTaskTypes",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HtBillingDetails_ActionById",
                table: "HtBillingDetails",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HtBillingDetails_BillId",
                table: "HtBillingDetails",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_HtBillingDetails_BookingRoomId",
                table: "HtBillingDetails",
                column: "BookingRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_HtBillingDetails_ServiceId",
                table: "HtBillingDetails",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_HtBillingDetails_UpdatedById",
                table: "HtBillingDetails",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HtBillings_ActionById",
                table: "HtBillings",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HtBillings_BillById",
                table: "HtBillings",
                column: "BillById");

            migrationBuilder.CreateIndex(
                name: "IX_HtBillings_BookingServiceId",
                table: "HtBillings",
                column: "BookingServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_HtBillings_UpdatedById",
                table: "HtBillings",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HtServices_ActionById",
                table: "HtServices",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HtServices_UpdatedById",
                table: "HtServices",
                column: "UpdatedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HkRoomAssigns");

            migrationBuilder.DropTable(
                name: "HkTaskAssigns");

            migrationBuilder.DropTable(
                name: "HkTaskRates");

            migrationBuilder.DropTable(
                name: "HtBillingDetails");

            migrationBuilder.DropTable(
                name: "HkTaskNames");

            migrationBuilder.DropTable(
                name: "HtBillings");

            migrationBuilder.DropTable(
                name: "HtServices");

            migrationBuilder.DropTable(
                name: "HkTaskTypes");
        }
    }
}
