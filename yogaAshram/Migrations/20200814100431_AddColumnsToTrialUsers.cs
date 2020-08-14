using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace yogaAshram.Migrations
{
    public partial class AddColumnsToTrialUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Commentdate",
                table: "TrialUserses",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "FreeLessons",
                table: "TrialUserses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SellerComment",
                table: "TrialUserses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Commentdate",
                table: "TrialUserses");

            migrationBuilder.DropColumn(
                name: "FreeLessons",
                table: "TrialUserses");

            migrationBuilder.DropColumn(
                name: "SellerComment",
                table: "TrialUserses");
        }
    }
}
