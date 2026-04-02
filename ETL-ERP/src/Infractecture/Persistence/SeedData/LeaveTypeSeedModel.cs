using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.SeedData
{
    public class LeaveTypeSeedModel
    {
        public static void Up(MigrationBuilder migrationBuilder)
        {
            LeaveTypeSeedData.Up(migrationBuilder);
        }

        public static void Down(MigrationBuilder migrationBuilder)
        {
            LeaveTypeSeedData.Down(migrationBuilder);
        }
    }
}
