using Microsoft.EntityFrameworkCore.Migrations;
using Persistence.SeedData;

#nullable disable

namespace Persistence.Migrations
{
    public partial class SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            BaseSeedModel.Up(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            BaseSeedModel.Down(migrationBuilder);
        }
    }
}
