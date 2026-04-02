using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class Transaction_Audit_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AuditById",
                table: "TranMsts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AuditDate",
                table: "TranMsts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuditRemarks",
                table: "TranMsts",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AuditById",
                table: "RsOrderPayments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AuditDate",
                table: "RsOrderPayments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuditRemarks",
                table: "RsOrderPayments",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AuditById",
                table: "RsFoodOrders",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AuditDate",
                table: "RsFoodOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuditRemarks",
                table: "RsFoodOrders",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AuditById",
                table: "HtBookingPayments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AuditDate",
                table: "HtBookingPayments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuditRemarks",
                table: "HtBookingPayments",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AuditById",
                table: "HtBookingHalls",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AuditDate",
                table: "HtBookingHalls",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuditRemarks",
                table: "HtBookingHalls",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AuditById",
                table: "HtBillingDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AuditDate",
                table: "HtBillingDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuditRemarks",
                table: "HtBillingDetails",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ServiceDate",
                table: "HtBillingDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TranMsts_AuditById",
                table: "TranMsts",
                column: "AuditById");

            migrationBuilder.CreateIndex(
                name: "IX_RsOrderPayments_AuditById",
                table: "RsOrderPayments",
                column: "AuditById");

            migrationBuilder.CreateIndex(
                name: "IX_RsFoodOrders_AuditById",
                table: "RsFoodOrders",
                column: "AuditById");

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingPayments_AuditById",
                table: "HtBookingPayments",
                column: "AuditById");

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingHalls_AuditById",
                table: "HtBookingHalls",
                column: "AuditById");

            migrationBuilder.CreateIndex(
                name: "IX_HtBillingDetails_AuditById",
                table: "HtBillingDetails",
                column: "AuditById");

            migrationBuilder.AddForeignKey(
                name: "FK_HtBillingDetails_AspNetUsers_AuditById",
                table: "HtBillingDetails",
                column: "AuditById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HtBookingHalls_AspNetUsers_AuditById",
                table: "HtBookingHalls",
                column: "AuditById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HtBookingPayments_AspNetUsers_AuditById",
                table: "HtBookingPayments",
                column: "AuditById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RsFoodOrders_AspNetUsers_AuditById",
                table: "RsFoodOrders",
                column: "AuditById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RsOrderPayments_AspNetUsers_AuditById",
                table: "RsOrderPayments",
                column: "AuditById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TranMsts_AspNetUsers_AuditById",
                table: "TranMsts",
                column: "AuditById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HtBillingDetails_AspNetUsers_AuditById",
                table: "HtBillingDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_HtBookingHalls_AspNetUsers_AuditById",
                table: "HtBookingHalls");

            migrationBuilder.DropForeignKey(
                name: "FK_HtBookingPayments_AspNetUsers_AuditById",
                table: "HtBookingPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_RsFoodOrders_AspNetUsers_AuditById",
                table: "RsFoodOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_RsOrderPayments_AspNetUsers_AuditById",
                table: "RsOrderPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_TranMsts_AspNetUsers_AuditById",
                table: "TranMsts");

            migrationBuilder.DropIndex(
                name: "IX_TranMsts_AuditById",
                table: "TranMsts");

            migrationBuilder.DropIndex(
                name: "IX_RsOrderPayments_AuditById",
                table: "RsOrderPayments");

            migrationBuilder.DropIndex(
                name: "IX_RsFoodOrders_AuditById",
                table: "RsFoodOrders");

            migrationBuilder.DropIndex(
                name: "IX_HtBookingPayments_AuditById",
                table: "HtBookingPayments");

            migrationBuilder.DropIndex(
                name: "IX_HtBookingHalls_AuditById",
                table: "HtBookingHalls");

            migrationBuilder.DropIndex(
                name: "IX_HtBillingDetails_AuditById",
                table: "HtBillingDetails");

            migrationBuilder.DropColumn(
                name: "AuditById",
                table: "TranMsts");

            migrationBuilder.DropColumn(
                name: "AuditDate",
                table: "TranMsts");

            migrationBuilder.DropColumn(
                name: "AuditRemarks",
                table: "TranMsts");

            migrationBuilder.DropColumn(
                name: "AuditById",
                table: "RsOrderPayments");

            migrationBuilder.DropColumn(
                name: "AuditDate",
                table: "RsOrderPayments");

            migrationBuilder.DropColumn(
                name: "AuditRemarks",
                table: "RsOrderPayments");

            migrationBuilder.DropColumn(
                name: "AuditById",
                table: "RsFoodOrders");

            migrationBuilder.DropColumn(
                name: "AuditDate",
                table: "RsFoodOrders");

            migrationBuilder.DropColumn(
                name: "AuditRemarks",
                table: "RsFoodOrders");

            migrationBuilder.DropColumn(
                name: "AuditById",
                table: "HtBookingPayments");

            migrationBuilder.DropColumn(
                name: "AuditDate",
                table: "HtBookingPayments");

            migrationBuilder.DropColumn(
                name: "AuditRemarks",
                table: "HtBookingPayments");

            migrationBuilder.DropColumn(
                name: "AuditById",
                table: "HtBookingHalls");

            migrationBuilder.DropColumn(
                name: "AuditDate",
                table: "HtBookingHalls");

            migrationBuilder.DropColumn(
                name: "AuditRemarks",
                table: "HtBookingHalls");

            migrationBuilder.DropColumn(
                name: "AuditById",
                table: "HtBillingDetails");

            migrationBuilder.DropColumn(
                name: "AuditDate",
                table: "HtBillingDetails");

            migrationBuilder.DropColumn(
                name: "AuditRemarks",
                table: "HtBillingDetails");

            migrationBuilder.DropColumn(
                name: "ServiceDate",
                table: "HtBillingDetails");
        }
    }
}
