using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class EmployeeDutyShiftAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DutyShiftId",
                table: "Employees",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DutyShiftId",
                table: "Employees",
                column: "DutyShiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_DutyShifts_DutyShiftId",
                table: "Employees",
                column: "DutyShiftId",
                principalTable: "DutyShifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_DutyShifts_DutyShiftId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_DutyShiftId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DutyShiftId",
                table: "Employees");
        }
    }
}
