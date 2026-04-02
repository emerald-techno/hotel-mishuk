using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.SeedData;

public class BaseSeedModel
{
    public static void Up(MigrationBuilder migrationBuilder)
    {
        UserSeedData.Up(migrationBuilder);
    }

    public static void Down(MigrationBuilder migrationBuilder)
    {
        UserSeedData.Up(migrationBuilder);
    }
}
