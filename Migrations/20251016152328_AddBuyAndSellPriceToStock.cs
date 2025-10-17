using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyGames.Migrations
{
    /// <inheritdoc />
    public partial class AddBuyAndSellPriceToStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Stocks",
                newName: "Source");

            migrationBuilder.AddColumn<decimal>(
                name: "BuyPrice",
                table: "Stocks",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SellPrice",
                table: "Stocks",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyPrice",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "SellPrice",
                table: "Stocks");

            migrationBuilder.RenameColumn(
                name: "Source",
                table: "Stocks",
                newName: "Price");
        }
    }
}
