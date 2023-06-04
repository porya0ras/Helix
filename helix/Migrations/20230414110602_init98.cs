using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace helix.Migrations
{
    public partial class init98 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DEC0",
                table: "SObjects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DEC1",
                table: "SObjects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DEC2",
                table: "SObjects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RA0",
                table: "SObjects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RA1",
                table: "SObjects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RA2",
                table: "SObjects",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DEC0",
                table: "SObjects");

            migrationBuilder.DropColumn(
                name: "DEC1",
                table: "SObjects");

            migrationBuilder.DropColumn(
                name: "DEC2",
                table: "SObjects");

            migrationBuilder.DropColumn(
                name: "RA0",
                table: "SObjects");

            migrationBuilder.DropColumn(
                name: "RA1",
                table: "SObjects");

            migrationBuilder.DropColumn(
                name: "RA2",
                table: "SObjects");
        }
    }
}
