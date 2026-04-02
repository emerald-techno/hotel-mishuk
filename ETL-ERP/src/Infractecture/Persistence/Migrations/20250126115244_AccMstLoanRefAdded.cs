using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class AccMstLoanRefAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EmpLoanDtlId",
                table: "AccTranMsts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EmpLoanMstId",
                table: "AccTranMsts",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccTranMsts_EmpLoanDtlId",
                table: "AccTranMsts",
                column: "EmpLoanDtlId");

            migrationBuilder.CreateIndex(
                name: "IX_AccTranMsts_EmpLoanMstId",
                table: "AccTranMsts",
                column: "EmpLoanMstId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccTranMsts_EmpLoanDtls_EmpLoanDtlId",
                table: "AccTranMsts",
                column: "EmpLoanDtlId",
                principalTable: "EmpLoanDtls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AccTranMsts_EmpLoanMsts_EmpLoanMstId",
                table: "AccTranMsts",
                column: "EmpLoanMstId",
                principalTable: "EmpLoanMsts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccTranMsts_EmpLoanDtls_EmpLoanDtlId",
                table: "AccTranMsts");

            migrationBuilder.DropForeignKey(
                name: "FK_AccTranMsts_EmpLoanMsts_EmpLoanMstId",
                table: "AccTranMsts");

            migrationBuilder.DropIndex(
                name: "IX_AccTranMsts_EmpLoanDtlId",
                table: "AccTranMsts");

            migrationBuilder.DropIndex(
                name: "IX_AccTranMsts_EmpLoanMstId",
                table: "AccTranMsts");

            migrationBuilder.DropColumn(
                name: "EmpLoanDtlId",
                table: "AccTranMsts");

            migrationBuilder.DropColumn(
                name: "EmpLoanMstId",
                table: "AccTranMsts");
        }
    }
}
