using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class HallBookingEntityAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BookingType",
                table: "HtBookingServices",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BookingHallId",
                table: "HtBillingDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HtHallInfos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HallName = table.Column<string>(type: "VARCHAR(25)", maxLength: 25, nullable: false),
                    HallInformation = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    OtherInfo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Rent = table.Column<double>(type: "float", nullable: false),
                    Vat = table.Column<double>(type: "float", nullable: false),
                    ServiceCharge = table.Column<double>(type: "float", nullable: false),
                    TotalRent = table.Column<double>(type: "float", nullable: false),
                    Remakrs = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    PhotoUrl = table.Column<string>(type: "VARCHAR(120)", maxLength: 120, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtHallInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtHallInfos_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtHallInfos_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HtBookingHalls",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HallShift = table.Column<int>(type: "int", nullable: false),
                    HallRent = table.Column<double>(type: "float", nullable: false),
                    Rent = table.Column<double>(type: "float", nullable: false),
                    ServiceCharge = table.Column<double>(type: "float", nullable: false),
                    Vat = table.Column<double>(type: "float", nullable: false),
                    Tax = table.Column<double>(type: "float", nullable: false),
                    Discount = table.Column<double>(type: "float", nullable: false),
                    NetRent = table.Column<double>(type: "float", nullable: false),
                    TotalPerson = table.Column<double>(type: "float", nullable: false),
                    Remarks = table.Column<string>(type: "VARCHAR(150)", maxLength: 150, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    BookingId = table.Column<long>(type: "bigint", nullable: false),
                    HallId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtBookingHalls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtBookingHalls_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtBookingHalls_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtBookingHalls_HtBookingServices_BookingId",
                        column: x => x.BookingId,
                        principalTable: "HtBookingServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtBookingHalls_HtHallInfos_HallId",
                        column: x => x.HallId,
                        principalTable: "HtHallInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HtBillingDetails_BookingHallId",
                table: "HtBillingDetails",
                column: "BookingHallId");

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingHalls_ActionById",
                table: "HtBookingHalls",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingHalls_BookingId",
                table: "HtBookingHalls",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingHalls_HallId",
                table: "HtBookingHalls",
                column: "HallId");

            migrationBuilder.CreateIndex(
                name: "IX_HtBookingHalls_UpdatedById",
                table: "HtBookingHalls",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HtHallInfos_ActionById",
                table: "HtHallInfos",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HtHallInfos_UpdatedById",
                table: "HtHallInfos",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_HtBillingDetails_HtBookingHalls_BookingHallId",
                table: "HtBillingDetails",
                column: "BookingHallId",
                principalTable: "HtBookingHalls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HtBillingDetails_HtBookingHalls_BookingHallId",
                table: "HtBillingDetails");

            migrationBuilder.DropTable(
                name: "HtBookingHalls");

            migrationBuilder.DropTable(
                name: "HtHallInfos");

            migrationBuilder.DropIndex(
                name: "IX_HtBillingDetails_BookingHallId",
                table: "HtBillingDetails");

            migrationBuilder.DropColumn(
                name: "BookingHallId",
                table: "HtBillingDetails");

            migrationBuilder.AlterColumn<string>(
                name: "BookingType",
                table: "HtBookingServices",
                type: "NVARCHAR(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1)",
                oldMaxLength: 1,
                oldNullable: true);
        }
    }
}
