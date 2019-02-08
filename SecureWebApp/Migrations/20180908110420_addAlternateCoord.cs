using Microsoft.EntityFrameworkCore.Migrations;

namespace SALUSUAVWebApp.Migrations
{
    public partial class addAlternateCoord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AlternateLat",
                table: "FlightData",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "AlternateLon",
                table: "FlightData",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlternateLat",
                table: "FlightData");

            migrationBuilder.DropColumn(
                name: "AlternateLon",
                table: "FlightData");
        }
    }
}
