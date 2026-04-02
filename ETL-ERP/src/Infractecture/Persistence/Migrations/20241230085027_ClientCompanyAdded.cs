using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class ClientCompanyAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Dob",
                table: "RsCustomers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "RsCustomers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "HtGuestInfos",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClientCompanies",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    WebSite = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ActionById = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientCompanies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientCompanies_AspNetUsers_ActionById",
                        column: x => x.ActionById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientCompanies_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RsCustomers_CompanyId",
                table: "RsCustomers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_HtGuestInfos_CompanyId",
                table: "HtGuestInfos",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientCompanies_ActionById",
                table: "ClientCompanies",
                column: "ActionById");

            migrationBuilder.CreateIndex(
                name: "IX_ClientCompanies_UpdatedById",
                table: "ClientCompanies",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_HtGuestInfos_ClientCompanies_CompanyId",
                table: "HtGuestInfos",
                column: "CompanyId",
                principalTable: "ClientCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RsCustomers_ClientCompanies_CompanyId",
                table: "RsCustomers",
                column: "CompanyId",
                principalTable: "ClientCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HtGuestInfos_ClientCompanies_CompanyId",
                table: "HtGuestInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_RsCustomers_ClientCompanies_CompanyId",
                table: "RsCustomers");

            migrationBuilder.DropTable(
                name: "ClientCompanies");

            migrationBuilder.DropIndex(
                name: "IX_RsCustomers_CompanyId",
                table: "RsCustomers");

            migrationBuilder.DropIndex(
                name: "IX_HtGuestInfos_CompanyId",
                table: "HtGuestInfos");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "RsCustomers");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "HtGuestInfos");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Dob",
                table: "RsCustomers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
