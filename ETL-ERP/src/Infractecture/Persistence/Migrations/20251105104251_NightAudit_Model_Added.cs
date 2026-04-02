using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class NightAudit_Model_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HtBusinessDays",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AuditStatus = table.Column<short>(type: "smallint", nullable: false),
                    TotalRoomRevenue = table.Column<double>(type: "float", nullable: false),
                    TotalFoodAndBeverageRevenue = table.Column<double>(type: "float", nullable: false),
                    TotalHallRevenue = table.Column<double>(type: "float", nullable: false),
                    TotalOtherRevenue = table.Column<double>(type: "float", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtBusinessDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtBusinessDays_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HtDailyAuditSummaries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalRoomsAvailable = table.Column<int>(type: "int", nullable: false),
                    TotalRoomsOccupied = table.Column<int>(type: "int", nullable: false),
                    TotalRoomsVacant = table.Column<int>(type: "int", nullable: false),
                    TotalRoomsOutOfOrder = table.Column<int>(type: "int", nullable: false),
                    TotalRoomsHouseUse = table.Column<int>(type: "int", nullable: false),
                    TotalRoomMoves = table.Column<int>(type: "int", nullable: false),
                    TotalCheckIns = table.Column<int>(type: "int", nullable: false),
                    TotalCheckOuts = table.Column<int>(type: "int", nullable: false),
                    TotalNoShows = table.Column<int>(type: "int", nullable: false),
                    TotalAdults = table.Column<int>(type: "int", nullable: false),
                    TotalChildren = table.Column<int>(type: "int", nullable: false),
                    TotalRackRate = table.Column<double>(type: "float", nullable: false),
                    TotalRoomRevenue = table.Column<double>(type: "float", nullable: false),
                    TotalServiceCharge = table.Column<double>(type: "float", nullable: false),
                    TotalVat = table.Column<double>(type: "float", nullable: false),
                    TotalDiscounts = table.Column<double>(type: "float", nullable: false),
                    TotalComplimentary = table.Column<double>(type: "float", nullable: false),
                    TotalRoomChargePosted = table.Column<double>(type: "float", nullable: false),
                    TotalRestaurantRevenue = table.Column<double>(type: "float", nullable: false),
                    TotalFNBServiceCharge = table.Column<double>(type: "float", nullable: false),
                    TotalFNBVat = table.Column<double>(type: "float", nullable: false),
                    TotalFNBDiscounts = table.Column<double>(type: "float", nullable: false),
                    TotalRoomServiceRevenue = table.Column<double>(type: "float", nullable: false),
                    TotalBanquetRevenue = table.Column<double>(type: "float", nullable: false),
                    TotalOtherRevenue = table.Column<double>(type: "float", nullable: false),
                    TotalCashCollection = table.Column<double>(type: "float", nullable: false),
                    TotalCardCollection = table.Column<double>(type: "float", nullable: false),
                    TotalMbankingCollection = table.Column<double>(type: "float", nullable: false),
                    TotalCreditCollection = table.Column<double>(type: "float", nullable: false),
                    TotalAdvanceCollection = table.Column<double>(type: "float", nullable: false),
                    TotalReceivable = table.Column<double>(type: "float", nullable: false),
                    TotalPayable = table.Column<double>(type: "float", nullable: false),
                    OccupancyPercentage = table.Column<double>(type: "float", nullable: false),
                    ADR = table.Column<double>(type: "float", nullable: false),
                    ARR = table.Column<double>(type: "float", nullable: false),
                    RevPAR = table.Column<double>(type: "float", nullable: false),
                    AvgGuestsPerRoom = table.Column<double>(type: "float", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtDailyAuditSummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtDailyAuditSummaries_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HtRoomDayAudits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Adult = table.Column<int>(type: "int", nullable: false),
                    Child = table.Column<int>(type: "int", nullable: false),
                    IsStay = table.Column<bool>(type: "bit", nullable: false),
                    IsOccupied = table.Column<bool>(type: "bit", nullable: false),
                    IsCharged = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RackRate = table.Column<double>(type: "float", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    BaseRate = table.Column<double>(type: "float", nullable: false),
                    ServiceCharge = table.Column<double>(type: "float", nullable: false),
                    Vat = table.Column<double>(type: "float", nullable: false),
                    NetAmount = table.Column<double>(type: "float", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BookingRoomId = table.Column<long>(type: "bigint", nullable: false),
                    RoomId = table.Column<long>(type: "bigint", nullable: false),
                    RoomCategoryId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtRoomDayAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtRoomDayAudits_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtRoomDayAudits_HtBookingRooms_BookingRoomId",
                        column: x => x.BookingRoomId,
                        principalTable: "HtBookingRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtRoomDayAudits_HtRoomCategories_RoomCategoryId",
                        column: x => x.RoomCategoryId,
                        principalTable: "HtRoomCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtRoomDayAudits_HtRoomInfos_RoomId",
                        column: x => x.RoomId,
                        principalTable: "HtRoomInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HtBusinessDays_ActionById",
                table: "HtBusinessDays",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HtDailyAuditSummaries_ActionById",
                table: "HtDailyAuditSummaries",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomDayAudits_ActionById",
                table: "HtRoomDayAudits",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomDayAudits_BookingRoomId",
                table: "HtRoomDayAudits",
                column: "BookingRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomDayAudits_RoomCategoryId",
                table: "HtRoomDayAudits",
                column: "RoomCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_HtRoomDayAudits_RoomId",
                table: "HtRoomDayAudits",
                column: "RoomId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HtBusinessDays");

            migrationBuilder.DropTable(
                name: "HtDailyAuditSummaries");

            migrationBuilder.DropTable(
                name: "HtRoomDayAudits");
        }
    }
}
