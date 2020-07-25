using Microsoft.EntityFrameworkCore.Migrations;

namespace yogaAshram.Migrations
{
    public partial class AddedOnTimePasswordFieldToEmployee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OnTimePassword",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnTimePassword",
                table: "AspNetUsers");
        }
    }
}
