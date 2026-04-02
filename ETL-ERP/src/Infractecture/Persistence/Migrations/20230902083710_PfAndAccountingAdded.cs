using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class PfAndAccountingAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.CreateTable(
                name: "AccGroups",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    GroupCode = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    GroupShortCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    SlNo = table.Column<short>(type: "smallint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccGroups_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccGroups_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccReviewers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReviewerType = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    SlNo = table.Column<short>(type: "smallint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ReviewrId = table.Column<long>(type: "bigint", nullable: false),
                    ReviewerId = table.Column<long>(type: "bigint", nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccReviewers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccReviewers_AccReviewers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "AccReviewers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccReviewers_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccReviewers_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccTranMsts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VcDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VcType = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    VcNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Narration = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    TotalAmount = table.Column<double>(type: "float", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsAudited = table.Column<bool>(type: "bit", nullable: false),
                    IsBankClear = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AuditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BankClearDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsAuto = table.Column<bool>(type: "bit", nullable: false),
                    RefNo = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    SubVacType = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    CurrencyId = table.Column<long>(type: "bigint", nullable: false),
                    FinYearId = table.Column<long>(type: "bigint", nullable: false),
                    AuditedById = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    BankClearById = table.Column<long>(type: "bigint", nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccTranMsts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccTranMsts_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccTranMsts_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccTranMsts_AspNetUsers_AuditedById",
                        column: x => x.AuditedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccTranMsts_AspNetUsers_BankClearById",
                        column: x => x.BankClearById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccTranMsts_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccTranMsts_SetCurrencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "SetCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccTranMsts_SetFincYears_FinYearId",
                        column: x => x.FinYearId,
                        principalTable: "SetFincYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpLoanMsts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanPassDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoanPayDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoanAmount = table.Column<double>(type: "float", nullable: false),
                    InterestRate = table.Column<double>(type: "float", nullable: false),
                    FirstInsDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoanFileUrl = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    InsAmount = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpLoanMsts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpLoanMsts_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpLoanMsts_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpLoanMsts_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyAttendanceSheetMsts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<short>(type: "smallint", nullable: false),
                    Month = table.Column<short>(type: "smallint", nullable: false),
                    DateFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyAttendanceSheetMsts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthlyAttendanceSheetMsts_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MonthlyAttendanceSheetMsts_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NoticeInfos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoticeTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileUrl = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ReceiverType = table.Column<short>(type: "smallint", nullable: false),
                    DisplayStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisplayEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ReceiverId = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    SenderId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoticeInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NoticeInfos_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NoticeInfos_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NoticeInfos_AspNetUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NoticeInfos_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NoticeInfos_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PfBoardMembers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    MemberRole = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    SlNo = table.Column<short>(type: "smallint", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisableDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PfBoardMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PfBoardMembers_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PfBoardMembers_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PfBoardMembers_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PfFundOpennings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpenningDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PfStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmpCon = table.Column<double>(type: "float", nullable: false),
                    CompCon = table.Column<double>(type: "float", nullable: false),
                    Interest = table.Column<double>(type: "float", nullable: false),
                    TotalAmount = table.Column<double>(type: "float", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PfFundOpennings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PfFundOpennings_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PfFundOpennings_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PfFundOpennings_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PfSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PfSource = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    EmpCon = table.Column<double>(type: "float", nullable: false),
                    CompCon = table.Column<double>(type: "float", nullable: false),
                    MaturityMonth = table.Column<short>(type: "smallint", nullable: false),
                    LoanAfter = table.Column<short>(type: "smallint", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PfSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PfSettings_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PfSettlements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SettlementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PfStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PfEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    StatusDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmpCon = table.Column<double>(type: "float", nullable: false),
                    CompCon = table.Column<double>(type: "float", nullable: false),
                    Interest = table.Column<double>(type: "float", nullable: false),
                    TotalAmount = table.Column<double>(type: "float", nullable: false),
                    Tax = table.Column<double>(type: "float", nullable: false),
                    Vat = table.Column<double>(type: "float", nullable: false),
                    NetAmount = table.Column<double>(type: "float", nullable: false),
                    FileDocUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApproveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    PfLength = table.Column<short>(type: "smallint", nullable: false),
                    CompConType = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PfSettlements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PfSettlements_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PfSettlements_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PfSettlements_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrArrearMsts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<short>(type: "smallint", nullable: false),
                    Month = table.Column<short>(type: "smallint", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedRemarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrArrearMsts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrArrearMsts_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrArrearMsts_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrArrearMsts_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrGuestSalaryMsts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<short>(type: "smallint", nullable: false),
                    Month = table.Column<short>(type: "smallint", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedRemarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrGuestSalaryMsts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrGuestSalaryMsts_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrGuestSalaryMsts_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrGuestSalaryMsts_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrSalaryMsts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalaryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Year = table.Column<short>(type: "smallint", nullable: false),
                    Month = table.Column<short>(type: "smallint", nullable: false),
                    DateFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorkingDays = table.Column<short>(type: "smallint", nullable: false),
                    TotalEmployee = table.Column<short>(type: "smallint", nullable: false),
                    TotalSalary = table.Column<double>(type: "float", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ApprovedRemarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedById = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrSalaryMsts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrSalaryMsts_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrSalaryMsts_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrSalaryMsts_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrSalaryParts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    PartCode = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    PartType = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    ValueType = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    PartLink = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    Value = table.Column<double>(type: "float", nullable: false),
                    IsEmpWise = table.Column<bool>(type: "bit", nullable: false),
                    IsEnable = table.Column<bool>(type: "bit", nullable: false),
                    SlNo = table.Column<short>(type: "smallint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrSalaryParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrSalaryParts_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrSalaryParts_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SetSalaryGrades",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GradeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartingBasic = table.Column<double>(type: "float", nullable: false),
                    MaxAmount = table.Column<double>(type: "float", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetSalaryGrades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SetSalaryGrades_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SetSalaryGrades_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccHeads",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentHeadId = table.Column<long>(type: "bigint", nullable: false),
                    LevelId = table.Column<int>(type: "int", nullable: false),
                    HeadName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    HeadCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    BudgetHead = table.Column<bool>(type: "bit", nullable: false),
                    AssetHead = table.Column<bool>(type: "bit", nullable: false),
                    PettyHead = table.Column<bool>(type: "bit", nullable: false),
                    BankHead = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    HeadGroupCode = table.Column<long>(type: "bigint", nullable: false),
                    GroupId = table.Column<long>(type: "bigint", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccHeads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccHeads_AccGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "AccGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccHeads_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccHeads_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccTranFiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadFile = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    SlNo = table.Column<short>(type: "smallint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    TranMstId = table.Column<long>(type: "bigint", nullable: false),
                    UploadById = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccTranFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccTranFiles_AccTranMsts_TranMstId",
                        column: x => x.TranMstId,
                        principalTable: "AccTranMsts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccTranFiles_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccTranFiles_AspNetUsers_UploadById",
                        column: x => x.UploadById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccTranNotes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoteDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoteType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NoteDesc = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    SlNo = table.Column<short>(type: "smallint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    TranMstId = table.Column<long>(type: "bigint", nullable: false),
                    NoteById = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccTranNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccTranNotes_AccTranMsts_TranMstId",
                        column: x => x.TranMstId,
                        principalTable: "AccTranMsts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccTranNotes_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccTranNotes_AspNetUsers_NoteById",
                        column: x => x.NoteById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyAttendanceSheetDtls",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PresentDays = table.Column<short>(type: "smallint", nullable: false),
                    AbsentDays = table.Column<short>(type: "smallint", nullable: false),
                    LeaveDays = table.Column<short>(type: "smallint", nullable: false),
                    UnPaidLeaveDays = table.Column<short>(type: "smallint", nullable: false),
                    OffDays = table.Column<short>(type: "smallint", nullable: false),
                    Holidays = table.Column<short>(type: "smallint", nullable: false),
                    LateDays = table.Column<short>(type: "smallint", nullable: false),
                    PayDays = table.Column<short>(type: "smallint", nullable: false),
                    TotalDays = table.Column<short>(type: "smallint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    SheetId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyAttendanceSheetDtls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthlyAttendanceSheetDtls_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MonthlyAttendanceSheetDtls_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MonthlyAttendanceSheetDtls_MonthlyAttendanceSheetMsts_SheetId",
                        column: x => x.SheetId,
                        principalTable: "MonthlyAttendanceSheetMsts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrGuestSalaryDtls",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    GuestSalaryMstId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrGuestSalaryDtls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrGuestSalaryDtls_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrGuestSalaryDtls_PrGuestSalaryMsts_GuestSalaryMstId",
                        column: x => x.GuestSalaryMstId,
                        principalTable: "PrGuestSalaryMsts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PfFundMsts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FundType = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    FundFromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FundToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalEmpCon = table.Column<double>(type: "float", nullable: false),
                    TotalCompCon = table.Column<double>(type: "float", nullable: false),
                    Interest = table.Column<double>(type: "float", nullable: false),
                    EmpConPer = table.Column<double>(type: "float", nullable: false),
                    CompConPer = table.Column<double>(type: "float", nullable: false),
                    PfAmount = table.Column<double>(type: "float", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    VoucherId = table.Column<long>(type: "bigint", nullable: true),
                    PrMstId = table.Column<long>(type: "bigint", nullable: true),
                    InvestmentId = table.Column<long>(type: "bigint", nullable: true),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PfFundMsts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PfFundMsts_AccTranMsts_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "AccTranMsts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PfFundMsts_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PfFundMsts_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PfFundMsts_PrSalaryMsts_PrMstId",
                        column: x => x.PrMstId,
                        principalTable: "PrSalaryMsts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrSalaryDtls",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GrossSalary = table.Column<double>(type: "float", nullable: false),
                    NetSalary = table.Column<double>(type: "float", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    IsHeldUp = table.Column<bool>(type: "bit", nullable: false),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BasicSalary = table.Column<double>(type: "float", nullable: false),
                    TotalDeduction = table.Column<double>(type: "float", nullable: false),
                    ColA = table.Column<double>(type: "float", nullable: false),
                    ColB = table.Column<double>(type: "float", nullable: false),
                    ColC = table.Column<double>(type: "float", nullable: false),
                    ColD = table.Column<double>(type: "float", nullable: false),
                    ColE = table.Column<double>(type: "float", nullable: false),
                    ColF = table.Column<double>(type: "float", nullable: false),
                    ColG = table.Column<double>(type: "float", nullable: false),
                    ColH = table.Column<double>(type: "float", nullable: false),
                    ColI = table.Column<double>(type: "float", nullable: false),
                    ColJ = table.Column<double>(type: "float", nullable: false),
                    ColK = table.Column<double>(type: "float", nullable: false),
                    ColL = table.Column<double>(type: "float", nullable: false),
                    ColM = table.Column<double>(type: "float", nullable: false),
                    ColN = table.Column<double>(type: "float", nullable: false),
                    ColO = table.Column<double>(type: "float", nullable: false),
                    ColP = table.Column<double>(type: "float", nullable: false),
                    ColQ = table.Column<double>(type: "float", nullable: false),
                    ColR = table.Column<double>(type: "float", nullable: false),
                    ColS = table.Column<double>(type: "float", nullable: false),
                    ColT = table.Column<double>(type: "float", nullable: false),
                    ColU = table.Column<double>(type: "float", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    SalaryMstId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    PaidById = table.Column<long>(type: "bigint", nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrSalaryDtls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrSalaryDtls_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrSalaryDtls_AspNetUsers_PaidById",
                        column: x => x.PaidById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrSalaryDtls_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrSalaryDtls_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrSalaryDtls_PrSalaryMsts_SalaryMstId",
                        column: x => x.SalaryMstId,
                        principalTable: "PrSalaryMsts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrEmpSalaryParts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartType = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    ValueType = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    SalaryPartId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrEmpSalaryParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrEmpSalaryParts_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrEmpSalaryParts_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrEmpSalaryParts_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrEmpSalaryParts_PrSalaryParts_SalaryPartId",
                        column: x => x.SalaryPartId,
                        principalTable: "PrSalaryParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccLedgers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LedgerName = table.Column<string>(type: "nvarchar(90)", maxLength: 90, nullable: false),
                    LedgerCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    LedgerGroupCode = table.Column<long>(type: "bigint", nullable: false),
                    HeadId = table.Column<long>(type: "bigint", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccLedgers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccLedgers_AccHeads_HeadId",
                        column: x => x.HeadId,
                        principalTable: "AccHeads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccLedgers_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccLedgers_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PfFundDtls",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpCon = table.Column<double>(type: "float", nullable: false),
                    CompCon = table.Column<double>(type: "float", nullable: false),
                    Interest = table.Column<double>(type: "float", nullable: false),
                    TotalAmount = table.Column<double>(type: "float", nullable: false),
                    EmpSalary = table.Column<double>(type: "float", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    FundMstId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PfFundDtls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PfFundDtls_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PfFundDtls_PfFundMsts_FundMstId",
                        column: x => x.FundMstId,
                        principalTable: "PfFundMsts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpLoanDtls",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InsAmount = table.Column<double>(type: "float", nullable: false),
                    InterestAmount = table.Column<double>(type: "float", nullable: false),
                    LoanAmount = table.Column<double>(type: "float", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    Serial = table.Column<short>(type: "smallint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LoanId = table.Column<long>(type: "bigint", nullable: false),
                    PrDtlId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpLoanDtls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpLoanDtls_EmpLoanMsts_LoanId",
                        column: x => x.LoanId,
                        principalTable: "EmpLoanMsts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpLoanDtls_PrSalaryDtls_PrDtlId",
                        column: x => x.PrDtlId,
                        principalTable: "PrSalaryDtls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrArrearDtls",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    ArrearFor = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ArrearId = table.Column<long>(type: "bigint", nullable: false),
                    EmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    SalaryId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrArrearDtls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrArrearDtls_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrArrearDtls_PrArrearMsts_ArrearId",
                        column: x => x.ArrearId,
                        principalTable: "PrArrearMsts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrArrearDtls_PrSalaryDtls_SalaryId",
                        column: x => x.SalaryId,
                        principalTable: "PrSalaryDtls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccTranDtls",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AmountDr = table.Column<double>(type: "float", nullable: false),
                    AmountCr = table.Column<double>(type: "float", nullable: false),
                    AmountUsdCr = table.Column<double>(type: "float", nullable: false),
                    AmountUsdDr = table.Column<double>(type: "float", nullable: false),
                    ChkNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChkDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SlNo = table.Column<short>(type: "smallint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    TranMstId = table.Column<long>(type: "bigint", nullable: false),
                    LedgerDrId = table.Column<long>(type: "bigint", nullable: true),
                    LedgerCrId = table.Column<long>(type: "bigint", nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccTranDtls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccTranDtls_AccLedgers_LedgerCrId",
                        column: x => x.LedgerCrId,
                        principalTable: "AccLedgers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccTranDtls_AccLedgers_LedgerDrId",
                        column: x => x.LedgerDrId,
                        principalTable: "AccLedgers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccTranDtls_AccTranMsts_TranMstId",
                        column: x => x.TranMstId,
                        principalTable: "AccTranMsts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccTranDtls_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccTranDtls_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccGroups_ActionById",
                table: "AccGroups",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_AccGroups_UpdatedById",
                table: "AccGroups",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccHeads_ActionById",
                table: "AccHeads",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_AccHeads_GroupId",
                table: "AccHeads",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AccHeads_UpdatedById",
                table: "AccHeads",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccLedgers_ActionById",
                table: "AccLedgers",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_AccLedgers_HeadId",
                table: "AccLedgers",
                column: "HeadId");

            migrationBuilder.CreateIndex(
                name: "IX_AccLedgers_UpdatedById",
                table: "AccLedgers",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccReviewers_ActionById",
                table: "AccReviewers",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_AccReviewers_ReviewerId",
                table: "AccReviewers",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccReviewers_UpdatedById",
                table: "AccReviewers",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccTranDtls_ActionById",
                table: "AccTranDtls",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_AccTranDtls_LedgerCrId",
                table: "AccTranDtls",
                column: "LedgerCrId");

            migrationBuilder.CreateIndex(
                name: "IX_AccTranDtls_LedgerDrId",
                table: "AccTranDtls",
                column: "LedgerDrId");

            migrationBuilder.CreateIndex(
                name: "IX_AccTranDtls_TranMstId",
                table: "AccTranDtls",
                column: "TranMstId");

            migrationBuilder.CreateIndex(
                name: "IX_AccTranDtls_UpdatedById",
                table: "AccTranDtls",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccTranFiles_ActionById",
                table: "AccTranFiles",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_AccTranFiles_TranMstId",
                table: "AccTranFiles",
                column: "TranMstId");

            migrationBuilder.CreateIndex(
                name: "IX_AccTranFiles_UploadById",
                table: "AccTranFiles",
                column: "UploadById");

            migrationBuilder.CreateIndex(
                name: "IX_AccTranMsts_ActionById",
                table: "AccTranMsts",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_AccTranMsts_ApprovedById",
                table: "AccTranMsts",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccTranMsts_AuditedById",
                table: "AccTranMsts",
                column: "AuditedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccTranMsts_BankClearById",
                table: "AccTranMsts",
                column: "BankClearById");

            migrationBuilder.CreateIndex(
                name: "IX_AccTranMsts_CurrencyId",
                table: "AccTranMsts",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccTranMsts_FinYearId",
                table: "AccTranMsts",
                column: "FinYearId");

            migrationBuilder.CreateIndex(
                name: "IX_AccTranMsts_UpdatedById",
                table: "AccTranMsts",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AccTranNotes_ActionById",
                table: "AccTranNotes",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_AccTranNotes_NoteById",
                table: "AccTranNotes",
                column: "NoteById");

            migrationBuilder.CreateIndex(
                name: "IX_AccTranNotes_TranMstId",
                table: "AccTranNotes",
                column: "TranMstId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpLoanDtls_LoanId",
                table: "EmpLoanDtls",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpLoanDtls_PrDtlId",
                table: "EmpLoanDtls",
                column: "PrDtlId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpLoanMsts_ActionById",
                table: "EmpLoanMsts",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_EmpLoanMsts_EmployeeId",
                table: "EmpLoanMsts",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpLoanMsts_UpdatedById",
                table: "EmpLoanMsts",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyAttendanceSheetDtls_ActionById",
                table: "MonthlyAttendanceSheetDtls",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyAttendanceSheetDtls_EmployeeId",
                table: "MonthlyAttendanceSheetDtls",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyAttendanceSheetDtls_SheetId",
                table: "MonthlyAttendanceSheetDtls",
                column: "SheetId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyAttendanceSheetMsts_ActionById",
                table: "MonthlyAttendanceSheetMsts",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyAttendanceSheetMsts_UpdatedById",
                table: "MonthlyAttendanceSheetMsts",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_NoticeInfos_ActionById",
                table: "NoticeInfos",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_NoticeInfos_ApprovedById",
                table: "NoticeInfos",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_NoticeInfos_ReceiverId",
                table: "NoticeInfos",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_NoticeInfos_SenderId",
                table: "NoticeInfos",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_NoticeInfos_UpdatedById",
                table: "NoticeInfos",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PfBoardMembers_ActionById",
                table: "PfBoardMembers",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_PfBoardMembers_EmployeeId",
                table: "PfBoardMembers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PfBoardMembers_UpdatedById",
                table: "PfBoardMembers",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PfFundDtls_EmployeeId",
                table: "PfFundDtls",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PfFundDtls_FundMstId",
                table: "PfFundDtls",
                column: "FundMstId");

            migrationBuilder.CreateIndex(
                name: "IX_PfFundMsts_ActionById",
                table: "PfFundMsts",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_PfFundMsts_PrMstId",
                table: "PfFundMsts",
                column: "PrMstId");

            migrationBuilder.CreateIndex(
                name: "IX_PfFundMsts_UpdatedById",
                table: "PfFundMsts",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PfFundMsts_VoucherId",
                table: "PfFundMsts",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_PfFundOpennings_ActionById",
                table: "PfFundOpennings",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_PfFundOpennings_EmployeeId",
                table: "PfFundOpennings",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PfFundOpennings_UpdatedById",
                table: "PfFundOpennings",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PfSettings_ActionById",
                table: "PfSettings",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_PfSettlements_ActionById",
                table: "PfSettlements",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_PfSettlements_EmployeeId",
                table: "PfSettlements",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PfSettlements_UpdatedById",
                table: "PfSettlements",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PrArrearDtls_ArrearId",
                table: "PrArrearDtls",
                column: "ArrearId");

            migrationBuilder.CreateIndex(
                name: "IX_PrArrearDtls_EmployeeId",
                table: "PrArrearDtls",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PrArrearDtls_SalaryId",
                table: "PrArrearDtls",
                column: "SalaryId");

            migrationBuilder.CreateIndex(
                name: "IX_PrArrearMsts_ActionById",
                table: "PrArrearMsts",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_PrArrearMsts_ApprovedById",
                table: "PrArrearMsts",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_PrArrearMsts_UpdatedById",
                table: "PrArrearMsts",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PrEmpSalaryParts_ActionById",
                table: "PrEmpSalaryParts",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_PrEmpSalaryParts_EmployeeId",
                table: "PrEmpSalaryParts",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PrEmpSalaryParts_SalaryPartId",
                table: "PrEmpSalaryParts",
                column: "SalaryPartId");

            migrationBuilder.CreateIndex(
                name: "IX_PrEmpSalaryParts_UpdatedById",
                table: "PrEmpSalaryParts",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PrGuestSalaryDtls_EmployeeId",
                table: "PrGuestSalaryDtls",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PrGuestSalaryDtls_GuestSalaryMstId",
                table: "PrGuestSalaryDtls",
                column: "GuestSalaryMstId");

            migrationBuilder.CreateIndex(
                name: "IX_PrGuestSalaryMsts_ActionById",
                table: "PrGuestSalaryMsts",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_PrGuestSalaryMsts_ApprovedById",
                table: "PrGuestSalaryMsts",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_PrGuestSalaryMsts_UpdatedById",
                table: "PrGuestSalaryMsts",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PrSalaryDtls_ActionById",
                table: "PrSalaryDtls",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_PrSalaryDtls_EmployeeId",
                table: "PrSalaryDtls",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PrSalaryDtls_PaidById",
                table: "PrSalaryDtls",
                column: "PaidById");

            migrationBuilder.CreateIndex(
                name: "IX_PrSalaryDtls_SalaryMstId",
                table: "PrSalaryDtls",
                column: "SalaryMstId");

            migrationBuilder.CreateIndex(
                name: "IX_PrSalaryDtls_UpdatedById",
                table: "PrSalaryDtls",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PrSalaryMsts_ActionById",
                table: "PrSalaryMsts",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_PrSalaryMsts_ApprovedById",
                table: "PrSalaryMsts",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_PrSalaryMsts_UpdatedById",
                table: "PrSalaryMsts",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PrSalaryParts_ActionById",
                table: "PrSalaryParts",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_PrSalaryParts_UpdatedById",
                table: "PrSalaryParts",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SetSalaryGrades_ActionById",
                table: "SetSalaryGrades",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_SetSalaryGrades_UpdatedById",
                table: "SetSalaryGrades",
                column: "UpdatedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccReviewers");

            migrationBuilder.DropTable(
                name: "AccTranDtls");

            migrationBuilder.DropTable(
                name: "AccTranFiles");

            migrationBuilder.DropTable(
                name: "AccTranNotes");

            migrationBuilder.DropTable(
                name: "EmpLoanDtls");

            migrationBuilder.DropTable(
                name: "MonthlyAttendanceSheetDtls");

            migrationBuilder.DropTable(
                name: "NoticeInfos");

            migrationBuilder.DropTable(
                name: "PfBoardMembers");

            migrationBuilder.DropTable(
                name: "PfFundDtls");

            migrationBuilder.DropTable(
                name: "PfFundOpennings");

            migrationBuilder.DropTable(
                name: "PfSettings");

            migrationBuilder.DropTable(
                name: "PfSettlements");

            migrationBuilder.DropTable(
                name: "PrArrearDtls");

            migrationBuilder.DropTable(
                name: "PrEmpSalaryParts");

            migrationBuilder.DropTable(
                name: "PrGuestSalaryDtls");

            migrationBuilder.DropTable(
                name: "SetSalaryGrades");

            migrationBuilder.DropTable(
                name: "AccLedgers");

            migrationBuilder.DropTable(
                name: "EmpLoanMsts");

            migrationBuilder.DropTable(
                name: "MonthlyAttendanceSheetMsts");

            migrationBuilder.DropTable(
                name: "PfFundMsts");

            migrationBuilder.DropTable(
                name: "PrArrearMsts");

            migrationBuilder.DropTable(
                name: "PrSalaryDtls");

            migrationBuilder.DropTable(
                name: "PrSalaryParts");

            migrationBuilder.DropTable(
                name: "PrGuestSalaryMsts");

            migrationBuilder.DropTable(
                name: "AccHeads");

            migrationBuilder.DropTable(
                name: "AccTranMsts");

            migrationBuilder.DropTable(
                name: "PrSalaryMsts");

            migrationBuilder.DropTable(
                name: "AccGroups");

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sessions_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_ActionById",
                table: "Sessions",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UpdatedById",
                table: "Sessions",
                column: "UpdatedById");
        }
    }
}
