using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class UpdateTranMstEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ReqMstId",
                table: "TranMsts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EmployeeId",
                table: "RsCustomers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "ExtraBed",
                table: "HtBookingRooms",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<double>(
                name: "ExtraBedCharge",
                table: "HtBookingRooms",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_TranMsts_ReqMstId",
                table: "TranMsts",
                column: "ReqMstId");

            migrationBuilder.CreateIndex(
                name: "IX_RsCustomers_EmployeeId",
                table: "RsCustomers",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RsCustomers_Employees_EmployeeId",
                table: "RsCustomers",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TranMsts_RequsitionInfos_ReqMstId",
                table: "TranMsts",
                column: "ReqMstId",
                principalTable: "RequsitionInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RsCustomers_Employees_EmployeeId",
                table: "RsCustomers");

            migrationBuilder.DropForeignKey(
                name: "FK_TranMsts_RequsitionInfos_ReqMstId",
                table: "TranMsts");

            migrationBuilder.DropIndex(
                name: "IX_TranMsts_ReqMstId",
                table: "TranMsts");

            migrationBuilder.DropIndex(
                name: "IX_RsCustomers_EmployeeId",
                table: "RsCustomers");

            migrationBuilder.DropColumn(
                name: "ReqMstId",
                table: "TranMsts");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "RsCustomers");

            migrationBuilder.DropColumn(
                name: "ExtraBed",
                table: "HtBookingRooms");

            migrationBuilder.DropColumn(
                name: "ExtraBedCharge",
                table: "HtBookingRooms");
        }
    }
}
