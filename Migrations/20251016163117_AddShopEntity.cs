using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyGames.Migrations
{
    /// <inheritdoc />
    public partial class AddShopEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Shops_ProprietorId",
                table: "Shops",
                column: "ProprietorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shops_Users_ProprietorId",
                table: "Shops",
                column: "ProprietorId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shops_Users_ProprietorId",
                table: "Shops");

            migrationBuilder.DropIndex(
                name: "IX_Shops_ProprietorId",
                table: "Shops");
        }
    }
}
