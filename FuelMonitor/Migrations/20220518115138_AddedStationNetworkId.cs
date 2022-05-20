using Microsoft.EntityFrameworkCore.Migrations;

namespace FuelMonitor.Migrations
{
    public partial class AddedStationNetworkId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NetworkId",
                table: "Stations",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NetworkId",
                table: "Stations");
        }
    }
}
