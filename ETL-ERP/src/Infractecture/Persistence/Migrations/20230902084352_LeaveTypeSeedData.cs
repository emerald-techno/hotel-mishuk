using Microsoft.EntityFrameworkCore.Migrations;
using Persistence.SeedData;

#nullable disable

namespace Persistence.Migrations
{
    public partial class LeaveTypeSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            LeaveTypeSeedModel.Up(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            LeaveTypeSeedModel.Down(migrationBuilder);
        }
    }
}
