using Microsoft.EntityFrameworkCore.Migrations;

namespace QSApp.Migrations
{
    public partial class numbergenerationforpatientsonregistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PatientGeneratedNumber",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatientGeneratedNumber",
                table: "Patients");
        }
    }
}
