using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class ShiftManagementAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShiftManagements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Month = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    PermanentShiftId = table.Column<long>(type: "bigint", nullable: false),
                    DutyShiftId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftManagements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftManagements_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShiftManagements_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShiftManagements_DutyShifts_DutyShiftId",
                        column: x => x.DutyShiftId,
                        principalTable: "DutyShifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShiftManagements_DutyShifts_PermanentShiftId",
                        column: x => x.PermanentShiftId,
                        principalTable: "DutyShifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShiftManagements_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftManagements_ActionById",
                table: "ShiftManagements",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftManagements_DutyShiftId",
                table: "ShiftManagements",
                column: "DutyShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftManagements_EmployeeId",
                table: "ShiftManagements",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftManagements_PermanentShiftId",
                table: "ShiftManagements",
                column: "PermanentShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftManagements_UpdatedById",
                table: "ShiftManagements",
                column: "UpdatedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShiftManagements");
        }
    }
}
