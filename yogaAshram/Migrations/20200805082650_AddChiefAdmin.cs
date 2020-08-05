using Microsoft.EntityFrameworkCore.Migrations;

namespace yogaAshram.Migrations
{
    public partial class AddChiefAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<long>(
                name: "AdminId",
                table: "Branches",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branches_AdminId",
                table: "Branches",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Branches_AspNetUsers_AdminId",
                table: "Branches",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branches_AspNetUsers_AdminId",
                table: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Branches_AdminId",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Branches");

            migrationBuilder.AddColumn<long>(
                name: "MarketerId",
                table: "Branches",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SellerId",
                table: "Branches",
                type: "bigint",
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
    }
}
