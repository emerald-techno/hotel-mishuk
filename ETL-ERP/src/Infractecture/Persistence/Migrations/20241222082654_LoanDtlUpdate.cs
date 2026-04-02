using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class LoanDtlUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PaidDate",
                table: "EmpLoanDtls",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PaidSetById",
                table: "EmpLoanDtls",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "WaiverAmount",
                table: "EmpLoanDtls",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_EmpLoanDtls_PaidSetById",
                table: "EmpLoanDtls",
                column: "PaidSetById");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpLoanDtls_AspNetUsers_PaidSetById",
                table: "EmpLoanDtls",
                column: "PaidSetById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmpLoanDtls_AspNetUsers_PaidSetById",
                table: "EmpLoanDtls");

            migrationBuilder.DropIndex(
                name: "IX_EmpLoanDtls_PaidSetById",
                table: "EmpLoanDtls");

            migrationBuilder.DropColumn(
                name: "PaidDate",
                table: "EmpLoanDtls");

            migrationBuilder.DropColumn(
                name: "PaidSetById",
                table: "EmpLoanDtls");

            migrationBuilder.DropColumn(
                name: "WaiverAmount",
                table: "EmpLoanDtls");
        }
    }
}
