using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class BillUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CmpRemarks",
                table: "HtBillings",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ExtraBedCharge",
                table: "HtBillingDetails",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "IsComplimentary",
                table: "HtBillingDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "ServiceCharge",
                table: "HtBillingDetails",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CmpRemarks",
                table: "HtBillings");

            migrationBuilder.DropColumn(
                name: "ExtraBedCharge",
                table: "HtBillingDetails");

            migrationBuilder.DropColumn(
                name: "IsComplimentary",
                table: "HtBillingDetails");

            migrationBuilder.DropColumn(
                name: "ServiceCharge",
                table: "HtBillingDetails");
        }
    }
}
