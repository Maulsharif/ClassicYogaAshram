using Microsoft.EntityFrameworkCore.Migrations;

namespace yogaAshram.Migrations
{
    public partial class AddedPasswordStatesToEmployee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PasswordState",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordState",
                table: "AspNetUsers");
        }
    }
}
