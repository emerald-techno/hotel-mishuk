using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class AdvanceRefundAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HtAdvanceRefunds",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RefundDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RefundAmount = table.Column<double>(type: "float", nullable: false),
                    RefundMode = table.Column<int>(type: "int", nullable: false),
                    TransactionNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ChequeNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    AccountNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    BookingId = table.Column<long>(type: "bigint", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtAdvanceRefunds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HtAdvanceRefunds_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtAdvanceRefunds_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HtAdvanceRefunds_HtBookingServices_BookingId",
                        column: x => x.BookingId,
                        principalTable: "HtBookingServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HtAdvanceRefunds_ActionById",
                table: "HtAdvanceRefunds",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_HtAdvanceRefunds_BookingId",
                table: "HtAdvanceRefunds",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_HtAdvanceRefunds_UpdatedById",
                table: "HtAdvanceRefunds",
                column: "UpdatedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HtAdvanceRefunds");
        }
    }
}
