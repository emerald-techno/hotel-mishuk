using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class BillEntityUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmmount",
                table: "HtBillings",
                newName: "TotalAmount");

            migrationBuilder.AddColumn<double>(
                name: "PaidAmount",
                table: "HtBillings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "HtBillings");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "HtBillings",
                newName: "TotalAmmount");
        }
    }
}
