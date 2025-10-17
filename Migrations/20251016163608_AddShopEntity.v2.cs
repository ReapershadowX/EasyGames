using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyGames.Migrations
{
    /// <inheritdoc />
    public partial class AddShopEntityv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shops_Users_ProprietorId",
                table: "Shops");

            migrationBuilder.AddForeignKey(
                name: "FK_Shops_Users_ProprietorId",
                table: "Shops",
                column: "ProprietorId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shops_Users_ProprietorId",
                table: "Shops");

            migrationBuilder.AddForeignKey(
                name: "FK_Shops_Users_ProprietorId",
                table: "Shops",
                column: "ProprietorId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
