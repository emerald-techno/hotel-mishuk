using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class LeaveModule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DptLeaveReviewers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReviewFor = table.Column<short>(type: "smallint", nullable: false),
                    SlNo = table.Column<short>(type: "smallint", nullable: false),
                    IsFinalReviewer = table.Column<bool>(type: "bit", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DepartmentId = table.Column<long>(type: "bigint", nullable: false),
                    ReviewerId = table.Column<long>(type: "bigint", nullable: false),
                    AltReviewerId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DptLeaveReviewers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DptLeaveReviewers_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DptLeaveReviewers_AspNetUsers_AltReviewerId",
                        column: x => x.AltReviewerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DptLeaveReviewers_AspNetUsers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DptLeaveReviewers_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DptLeaveReviewers_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpAttendances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttendDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OutTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LunchOut = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LunchIn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    StatusDtl = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    IsManual = table.Column<bool>(type: "bit", nullable: false),
                    ManualEntrydate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsNight = table.Column<short>(type: "smallint", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    ManualEntryById = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpAttendances_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpAttendances_AspNetUsers_ManualEntryById",
                        column: x => x.ManualEntryById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpAttendances_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpLeaveReviewers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReviewFor = table.Column<short>(type: "smallint", nullable: false),
                    SlNo = table.Column<short>(type: "smallint", nullable: false),
                    IsFinalReviewer = table.Column<bool>(type: "bit", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    ReviewerId = table.Column<long>(type: "bigint", nullable: false),
                    AltReviewerId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpLeaveReviewers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpLeaveReviewers_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpLeaveReviewers_AspNetUsers_AltReviewerId",
                        column: x => x.AltReviewerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpLeaveReviewers_AspNetUsers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpLeaveReviewers_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpLeaveReviewers_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Balance = table.Column<int>(type: "int", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveTypes_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveTypes_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpLeaveApplications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AprFromDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AprToDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualFromDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualToDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubmitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TotalApprovalLeave = table.Column<short>(type: "smallint", nullable: false),
                    SalaryType = table.Column<short>(type: "smallint", nullable: false),
                    CancelDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelReason = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    FileDoc = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    LeaveTypeId = table.Column<long>(type: "bigint", nullable: false),
                    SubmitById = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpLeaveApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpLeaveApplications_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpLeaveApplications_AspNetUsers_SubmitById",
                        column: x => x.SubmitById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpLeaveApplications_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpLeaveApplications_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpLeaveApplications_LeaveTypes_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveCfs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeaveYear = table.Column<int>(type: "int", nullable: false),
                    LeaveBalance = table.Column<int>(type: "int", nullable: false),
                    CfBalance = table.Column<int>(type: "int", nullable: false),
                    LeaveEnjoyed = table.Column<int>(type: "int", nullable: false),
                    LeaveSale = table.Column<int>(type: "int", nullable: false),
                    LastActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LeaveTypeId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveCfs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveCfs_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveCfs_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveCfs_LeaveTypes_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeaveSetups",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeaveLimitType = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    LeaveBalance = table.Column<int>(type: "int", nullable: false),
                    MaxLeave = table.Column<int>(type: "int", nullable: false),
                    MinLeave = table.Column<int>(type: "int", nullable: false),
                    IsCarryForward = table.Column<bool>(type: "bit", nullable: false),
                    MaxCarryForward = table.Column<int>(type: "int", nullable: false),
                    LeaveAfter = table.Column<int>(type: "int", nullable: false),
                    ActiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LeaveTypeId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveSetups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveSetups_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveSetups_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveSetups_LeaveTypes_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LvAppReviewers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SlNo = table.Column<short>(type: "smallint", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReceiveTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResponseTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsFinalReviewer = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LeaveAppId = table.Column<long>(type: "bigint", nullable: false),
                    ReviewerId = table.Column<long>(type: "bigint", nullable: false),
                    AltReviewerId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LvAppReviewers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LvAppReviewers_AspNetUsers_AltReviewerId",
                        column: x => x.AltReviewerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LvAppReviewers_AspNetUsers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LvAppReviewers_EmpLeaveApplications_LeaveAppId",
                        column: x => x.LeaveAppId,
                        principalTable: "EmpLeaveApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LvTransferHsts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransferDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LvReviewerId = table.Column<long>(type: "bigint", nullable: false),
                    ReviewerId = table.Column<long>(type: "bigint", nullable: false),
                    PreReviewerId = table.Column<long>(type: "bigint", nullable: false),
                    TransferById = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LvTransferHsts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LvTransferHsts_AspNetUsers_PreReviewerId",
                        column: x => x.PreReviewerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LvTransferHsts_AspNetUsers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LvTransferHsts_AspNetUsers_TransferById",
                        column: x => x.TransferById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LvTransferHsts_LvAppReviewers_LvReviewerId",
                        column: x => x.LvReviewerId,
                        principalTable: "LvAppReviewers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DptLeaveReviewers_ActionById",
                table: "DptLeaveReviewers",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_DptLeaveReviewers_AltReviewerId",
                table: "DptLeaveReviewers",
                column: "AltReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_DptLeaveReviewers_DepartmentId",
                table: "DptLeaveReviewers",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DptLeaveReviewers_ReviewerId",
                table: "DptLeaveReviewers",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_DptLeaveReviewers_UpdatedById",
                table: "DptLeaveReviewers",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmpAttendances_ActionById",
                table: "EmpAttendances",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_EmpAttendances_EmployeeId",
                table: "EmpAttendances",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpAttendances_ManualEntryById",
                table: "EmpAttendances",
                column: "ManualEntryById");

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeaveApplications_ActionById",
                table: "EmpLeaveApplications",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeaveApplications_EmployeeId",
                table: "EmpLeaveApplications",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeaveApplications_LeaveTypeId",
                table: "EmpLeaveApplications",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeaveApplications_SubmitById",
                table: "EmpLeaveApplications",
                column: "SubmitById");

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeaveApplications_UpdatedById",
                table: "EmpLeaveApplications",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeaveReviewers_ActionById",
                table: "EmpLeaveReviewers",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeaveReviewers_AltReviewerId",
                table: "EmpLeaveReviewers",
                column: "AltReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeaveReviewers_EmployeeId",
                table: "EmpLeaveReviewers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeaveReviewers_ReviewerId",
                table: "EmpLeaveReviewers",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpLeaveReviewers_UpdatedById",
                table: "EmpLeaveReviewers",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveCfs_ActionById",
                table: "LeaveCfs",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveCfs_EmployeeId",
                table: "LeaveCfs",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveCfs_LeaveTypeId",
                table: "LeaveCfs",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveSetups_ActionById",
                table: "LeaveSetups",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveSetups_LeaveTypeId",
                table: "LeaveSetups",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveSetups_UpdatedById",
                table: "LeaveSetups",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveTypes_ActionById",
                table: "LeaveTypes",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveTypes_UpdatedById",
                table: "LeaveTypes",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LvAppReviewers_AltReviewerId",
                table: "LvAppReviewers",
                column: "AltReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_LvAppReviewers_LeaveAppId",
                table: "LvAppReviewers",
                column: "LeaveAppId");

            migrationBuilder.CreateIndex(
                name: "IX_LvAppReviewers_ReviewerId",
                table: "LvAppReviewers",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_LvTransferHsts_LvReviewerId",
                table: "LvTransferHsts",
                column: "LvReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_LvTransferHsts_PreReviewerId",
                table: "LvTransferHsts",
                column: "PreReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_LvTransferHsts_ReviewerId",
                table: "LvTransferHsts",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_LvTransferHsts_TransferById",
                table: "LvTransferHsts",
                column: "TransferById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DptLeaveReviewers");

            migrationBuilder.DropTable(
                name: "EmpAttendances");

            migrationBuilder.DropTable(
                name: "EmpLeaveReviewers");

            migrationBuilder.DropTable(
                name: "LeaveCfs");

            migrationBuilder.DropTable(
                name: "LeaveSetups");

            migrationBuilder.DropTable(
                name: "LvTransferHsts");

            migrationBuilder.DropTable(
                name: "LvAppReviewers");

            migrationBuilder.DropTable(
                name: "EmpLeaveApplications");

            migrationBuilder.DropTable(
                name: "LeaveTypes");
        }
    }
}
