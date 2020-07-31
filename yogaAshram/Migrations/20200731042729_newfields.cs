using Microsoft.EntityFrameworkCore.Migrations;

namespace yogaAshram.Migrations
{
    public partial class newfields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MarketerId",
                table: "Branches",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SellerId",
                table: "Branches",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branches_MarketerId",
                table: "Branches",
                column: "MarketerId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_SellerId",
                table: "Branches",
                column: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_AspNetUsers_MarketerId",
                table: "Branches",
                column: "MarketerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_AspNetUsers_SellerId",
                table: "Branches",
                column: "SellerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_AspNetUsers_MarketerId",
                table: "Branches");

            migrationBuilder.DropForeignKey(
                name: "FK_Branches_AspNetUsers_SellerId",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Branches_MarketerId",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Branches_SellerId",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "MarketerId",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "AspNetUsers");
        }
    }
}
