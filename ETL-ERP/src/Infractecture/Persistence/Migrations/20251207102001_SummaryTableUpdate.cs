using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class SummaryTableUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalVat",
                table: "HtDailyAuditSummaries",
                newName: "TotalRoomVat");

            migrationBuilder.RenameColumn(
                name: "TotalServiceCharge",
                table: "HtDailyAuditSummaries",
                newName: "TotalRoomServiceCharge");

            migrationBuilder.RenameColumn(
                name: "TotalDiscounts",
                table: "HtDailyAuditSummaries",
                newName: "TotalRoomDiscounts");

            migrationBuilder.RenameColumn(
                name: "TotalComplimentary",
                table: "HtDailyAuditSummaries",
                newName: "TotalRoomComplimentary");

            migrationBuilder.AddColumn<bool>(
                name: "IsExtra",
                table: "HtServices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "FrontOfficeAdvanceCover",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FrontOfficeBankCover",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FrontOfficeCardCover",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FrontOfficeCashCover",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FrontOfficeMbankingCover",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RestaurantAdvanceCover",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RestaurantBankCover",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RestaurantCardCover",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RestaurantCashCover",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RestaurantMbankingCover",
                table: "HtDailyAuditSummaries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "TotalRestaurantOrderAmount",
                table: "HtDailyAuditSummaries",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "BookingConfirmStatus",
                table: "HtBookingServices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExtra",
                table: "HtServices");

            migrationBuilder.DropColumn(
                name: "FrontOfficeAdvanceCover",
                table: "HtDailyAuditSummaries");

            migrationBuilder.DropColumn(
                name: "FrontOfficeBankCover",
                table: "HtDailyAuditSummaries");

            migrationBuilder.DropColumn(
                name: "FrontOfficeCardCover",
                table: "HtDailyAuditSummaries");

            migrationBuilder.DropColumn(
                name: "FrontOfficeCashCover",
                table: "HtDailyAuditSummaries");

            migrationBuilder.DropColumn(
                name: "FrontOfficeMbankingCover",
                table: "HtDailyAuditSummaries");

            migrationBuilder.DropColumn(
                name: "RestaurantAdvanceCover",
                table: "HtDailyAuditSummaries");

            migrationBuilder.DropColumn(
                name: "RestaurantBankCover",
                table: "HtDailyAuditSummaries");

            migrationBuilder.DropColumn(
                name: "RestaurantCardCover",
                table: "HtDailyAuditSummaries");

            migrationBuilder.DropColumn(
                name: "RestaurantCashCover",
                table: "HtDailyAuditSummaries");

            migrationBuilder.DropColumn(
                name: "RestaurantMbankingCover",
                table: "HtDailyAuditSummaries");

            migrationBuilder.DropColumn(
                name: "TotalRestaurantOrderAmount",
                table: "HtDailyAuditSummaries");

            migrationBuilder.DropColumn(
                name: "BookingConfirmStatus",
                table: "HtBookingServices");

            migrationBuilder.RenameColumn(
                name: "TotalRoomVat",
                table: "HtDailyAuditSummaries",
                newName: "TotalVat");

            migrationBuilder.RenameColumn(
                name: "TotalRoomServiceCharge",
                table: "HtDailyAuditSummaries",
                newName: "TotalServiceCharge");

            migrationBuilder.RenameColumn(
                name: "TotalRoomDiscounts",
                table: "HtDailyAuditSummaries",
                newName: "TotalDiscounts");

            migrationBuilder.RenameColumn(
                name: "TotalRoomComplimentary",
                table: "HtDailyAuditSummaries",
                newName: "TotalComplimentary");
        }
    }
}
