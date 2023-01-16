using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace helix.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObservationSubmissions_Frames__FrameId",
                table: "ObservationSubmissions");

            migrationBuilder.DropTable(
                name: "Frames");

            migrationBuilder.RenameColumn(
                name: "_FrameId",
                table: "ObservationSubmissions",
                newName: "_FilterId");

            migrationBuilder.RenameIndex(
                name: "IX_ObservationSubmissions__FrameId",
                table: "ObservationSubmissions",
                newName: "IX_ObservationSubmissions__FilterId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ObservationSubmissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ObservationSubmissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Filters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filters", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ObservationSubmissions_Filters__FilterId",
                table: "ObservationSubmissions",
                column: "_FilterId",
                principalTable: "Filters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObservationSubmissions_Filters__FilterId",
                table: "ObservationSubmissions");

            migrationBuilder.DropTable(
                name: "Filters");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ObservationSubmissions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ObservationSubmissions");

            migrationBuilder.RenameColumn(
                name: "_FilterId",
                table: "ObservationSubmissions",
                newName: "_FrameId");

            migrationBuilder.RenameIndex(
                name: "IX_ObservationSubmissions__FilterId",
                table: "ObservationSubmissions",
                newName: "IX_ObservationSubmissions__FrameId");

            migrationBuilder.CreateTable(
                name: "Frames",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Filter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Frames", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ObservationSubmissions_Frames__FrameId",
                table: "ObservationSubmissions",
                column: "_FrameId",
                principalTable: "Frames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
