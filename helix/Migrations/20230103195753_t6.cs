using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace helix.Migrations
{
    public partial class t6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey("PK_ObservationSubmissions", "ObservationSubmissions");

            migrationBuilder.DropColumn("Id", "ObservationSubmissions");
            migrationBuilder.AddColumn<Guid>("Id", "ObservationSubmissions");

            migrationBuilder.AddPrimaryKey("PK_ObservationSubmissions", "ObservationSubmissions","Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey("PK_ObservationSubmissions", "ObservationSubmissions");

            migrationBuilder.DropColumn("Id", "ObservationSubmissions");
            migrationBuilder.AddColumn<long>("Id", "ObservationSubmissions");

            migrationBuilder.AddPrimaryKey("PK_ObservationSubmissions", "ObservationSubmissions", "Id");
        }
    }
}
