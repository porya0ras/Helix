using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace helix.Migrations
{
    public partial class CombineCordinate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SObjects_Coordinates_CoordinateId",
                table: "SObjects");

            migrationBuilder.DropTable(
                name: "Coordinates");

            migrationBuilder.DropIndex(
                name: "IX_SObjects_CoordinateId",
                table: "SObjects");

            migrationBuilder.DropColumn(
                name: "CoordinateId",
                table: "SObjects");

            migrationBuilder.AddColumn<string>(
                name: "DEC",
                table: "SObjects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RA",
                table: "SObjects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DEC",
                table: "SObjects");

            migrationBuilder.DropColumn(
                name: "RA",
                table: "SObjects");

            migrationBuilder.AddColumn<long>(
                name: "CoordinateId",
                table: "SObjects",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Coordinates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DEC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RA = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coordinates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SObjects_CoordinateId",
                table: "SObjects",
                column: "CoordinateId");

            migrationBuilder.AddForeignKey(
                name: "FK_SObjects_Coordinates_CoordinateId",
                table: "SObjects",
                column: "CoordinateId",
                principalTable: "Coordinates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
