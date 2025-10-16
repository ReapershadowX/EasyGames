using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyGames.Migrations
{
    /// <inheritdoc />
    public partial class AddShopModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 9999);

            migrationBuilder.CreateTable(
                name: "Shops",
                columns: table => new
                {
                    ShopId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ProprietorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shops", x => x.ShopId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Shops");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedDate", "Email", "FirstName", "LastName", "Password", "Role" },
                values: new object[] { 9999, new DateTime(2025, 10, 16, 19, 45, 58, 826, DateTimeKind.Local).AddTicks(5654), "admin@email.com", "Emergency", "Admin", "AQAAAAIAAYagAAAAEKE363kE+0sL18ExxSoGvBUsBhD+jUaCp0eFWxCsDHe7jybsODoKSNM+LqgKf9AE3Q==", "Admin" });
        }
    }
}
