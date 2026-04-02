using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class PayModeDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdvance",
                table: "RsOrderPayments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "PayModeDetailId",
                table: "RsOrderPayments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PayModeDetailId",
                table: "InventoryBillPayments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalReceivable",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPayable",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalMbankingCollection",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalCreditCollection",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalCashCollection",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalCardCollection",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAdvanceCollection",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "RevPAR",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "OccupancyPercentage",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "AvgGuestsPerRoom",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "ARR",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "ADR",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalFrontOfficeAdvance",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalFrontOfficeBank",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalFrontOfficeCard",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalFrontOfficeCash",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalFrontOfficeMbanking",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalRestaurantAdvance",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalRestaurantBank",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalRestaurantCard",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalRestaurantCash",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalRestaurantMbanking",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdvance",
                table: "HtBookingPayments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "PayModeDetailId",
                table: "HtBookingPayments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PayModeDetailId",
                table: "HtAdvanceRefunds",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PayModeDetail",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    PayMode = table.Column<int>(type: "int", nullable: false),
                    ComRate = table.Column<double>(type: "float", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    PayLedgerId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayModeDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayModeDetail_AccLedgers_PayLedgerId",
                        column: x => x.PayLedgerId,
                        principalTable: "AccLedgers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PayModeDetail_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RsOrderPayments_PayModeDetailId",
                table: "RsOrderPayments",
                column: "PayModeDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryBillPayments_PayModeDetailId",
                table: "InventoryBillPayments",
                column: "PayModeDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingPayments_PayModeDetailId",
                table: "HtBookingPayments",
                column: "PayModeDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_HtAdvanceRefunds_PayModeDetailId",
                table: "HtAdvanceRefunds",
                column: "PayModeDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_PayModeDetail_ActionById",
                table: "PayModeDetail",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_PayModeDetail_PayLedgerId",
                table: "PayModeDetail",
                column: "PayLedgerId");

            migrationBuilder.AddForeignKey(
                name: "FK_HtAdvanceRefunds_PayModeDetail_PayModeDetailId",
                table: "HtAdvanceRefunds",
                column: "PayModeDetailId",
                principalTable: "PayModeDetail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HtBookingPayments_PayModeDetail_PayModeDetailId",
                table: "HtBookingPayments",
                column: "PayModeDetailId",
                principalTable: "PayModeDetail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryBillPayments_PayModeDetail_PayModeDetailId",
                table: "InventoryBillPayments",
                column: "PayModeDetailId",
                principalTable: "PayModeDetail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RsOrderPayments_PayModeDetail_PayModeDetailId",
                table: "RsOrderPayments",
                column: "PayModeDetailId",
                principalTable: "PayModeDetail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HtAdvanceRefunds_PayModeDetail_PayModeDetailId",
                table: "HtAdvanceRefunds");

            migrationBuilder.DropForeignKey(
                name: "FK_HtBookingPayments_PayModeDetail_PayModeDetailId",
                table: "HtBookingPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryBillPayments_PayModeDetail_PayModeDetailId",
                table: "InventoryBillPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_RsOrderPayments_PayModeDetail_PayModeDetailId",
                table: "RsOrderPayments");

            migrationBuilder.DropTable(
                name: "PayModeDetail");

            migrationBuilder.DropIndex(
                name: "IX_RsOrderPayments_PayModeDetailId",
                table: "RsOrderPayments");

            migrationBuilder.DropIndex(
                name: "IX_InventoryBillPayments_PayModeDetailId",
                table: "InventoryBillPayments");

            migrationBuilder.DropIndex(
                name: "IX_HtBookingPayments_PayModeDetailId",
                table: "HtBookingPayments");

            migrationBuilder.DropIndex(
                name: "IX_HtAdvanceRefunds_PayModeDetailId",
                table: "HtAdvanceRefunds");

            migrationBuilder.DropColumn(
                name: "IsAdvance",
                table: "RsOrderPayments");

            migrationBuilder.DropColumn(
                name: "PayModeDetailId",
                table: "RsOrderPayments");

            migrationBuilder.DropColumn(
                name: "PayModeDetailId",
                table: "InventoryBillPayments");

            migrationBuilder.DropColumn(
                name: "TotalFrontOfficeAdvance",
                table: "HtDailyAuditSummaries");

            migrationBuilder.DropColumn(
                name: "TotalFrontOfficeBank",
                table: "HtDailyAuditSummaries");

            migrationBuilder.DropColumn(
                name: "TotalFrontOfficeCard",
                table: "HtDailyAuditSummaries");

            migrationBuilder.DropColumn(
                name: "TotalFrontOfficeCash",
                table: "HtDailyAuditSummaries");

            migrationBuilder.DropColumn(
                name: "TotalFrontOfficeMbanking",
                table: "HtDailyAuditSummaries");

            migrationBuilder.DropColumn(
                name: "TotalRestaurantAdvance",
                table: "HtDailyAuditSummaries");

            migrationBuilder.DropColumn(
                name: "TotalRestaurantBank",
                table: "HtDailyAuditSummaries");

            migrationBuilder.DropColumn(
                name: "TotalRestaurantCard",
                table: "HtDailyAuditSummaries");

            migrationBuilder.DropColumn(
                name: "TotalRestaurantCash",
                table: "HtDailyAuditSummaries");

            migrationBuilder.DropColumn(
                name: "TotalRestaurantMbanking",
                table: "HtDailyAuditSummaries");

            migrationBuilder.DropColumn(
                name: "IsAdvance",
                table: "HtBookingPayments");

            migrationBuilder.DropColumn(
                name: "PayModeDetailId",
                table: "HtBookingPayments");

            migrationBuilder.DropColumn(
                name: "PayModeDetailId",
                table: "HtAdvanceRefunds");

            migrationBuilder.AlterColumn<double>(
                name: "TotalReceivable",
                table: "HtDailyAuditSummaries",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "TotalPayable",
                table: "HtDailyAuditSummaries",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "TotalMbankingCollection",
                table: "HtDailyAuditSummaries",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "TotalCreditCollection",
                table: "HtDailyAuditSummaries",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "TotalCashCollection",
                table: "HtDailyAuditSummaries",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "TotalCardCollection",
                table: "HtDailyAuditSummaries",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "TotalAdvanceCollection",
                table: "HtDailyAuditSummaries",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "RevPAR",
                table: "HtDailyAuditSummaries",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "OccupancyPercentage",
                table: "HtDailyAuditSummaries",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "AvgGuestsPerRoom",
                table: "HtDailyAuditSummaries",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "ARR",
                table: "HtDailyAuditSummaries",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "ADR",
                table: "HtDailyAuditSummaries",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
