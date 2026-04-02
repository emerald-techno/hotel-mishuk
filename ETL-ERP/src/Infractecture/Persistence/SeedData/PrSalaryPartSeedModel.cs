using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.SeedData
{
    public class PrSalaryPartSeedModel
    {
        public static void Up(MigrationBuilder migrationBuilder)
        {
            PrSalaryPartSeedData.Up(migrationBuilder);
        }

        public static void Down(MigrationBuilder migrationBuilder)
        {
            PrSalaryPartSeedData.Down(migrationBuilder);
        }
    }
}
