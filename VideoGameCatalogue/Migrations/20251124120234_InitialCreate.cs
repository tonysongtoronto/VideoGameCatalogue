using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VideoGameCatalogue.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VideoGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Publisher = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Genre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoGames", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "VideoGames",
                columns: new[] { "Id", "Genre", "Platform", "Price", "Publisher", "ReleaseDate", "Title" },
                values: new object[,]
                {
                    { 1, "Action-Adventure", "Nintendo Switch", 59.99m, "Nintendo", new DateTime(2017, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Legend of Zelda: Breath of the Wild" },
                    { 2, "Action RPG", "PC", 59.99m, "FromSoftware", new DateTime(2022, 2, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Elden Ring" },
                    { 3, "Action-Adventure", "PlayStation 5", 49.99m, "Sony Interactive Entertainment", new DateTime(2018, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "God of War" },
                    { 4, "RPG", "PC", 39.99m, "CD Projekt Red", new DateTime(2020, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cyberpunk 2077" },
                    { 5, "Roguelike", "Nintendo Switch", 24.99m, "Supergiant Games", new DateTime(2020, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hades" },
                    { 6, "Action-Adventure", "Xbox Series X", 59.99m, "Rockstar Games", new DateTime(2018, 10, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Red Dead Redemption 2" },
                    { 7, "Simulation", "PC", 14.99m, "ConcernedApe", new DateTime(2016, 2, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Stardew Valley" },
                    { 8, "Metroidvania", "PC", 14.99m, "Team Cherry", new DateTime(2017, 2, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hollow Knight" },
                    { 9, "RPG", "PC", 39.99m, "CD Projekt Red", new DateTime(2015, 5, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Witcher 3: Wild Hunt" },
                    { 10, "Sandbox", "PC", 26.95m, "Mojang Studios", new DateTime(2011, 11, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Minecraft" },
                    { 11, "Action RPG", "PlayStation 4", 39.99m, "FromSoftware", new DateTime(2016, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dark Souls III" },
                    { 12, "Platformer", "Nintendo Switch", 19.99m, "Maddy Makes Games", new DateTime(2018, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Celeste" },
                    { 13, "Puzzle", "PC", 9.99m, "Valve", new DateTime(2011, 4, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Portal 2" },
                    { 14, "Action-Adventure", "PC", 59.99m, "FromSoftware", new DateTime(2019, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sekiro: Shadows Die Twice" },
                    { 15, "Simulation", "Nintendo Switch", 59.99m, "Nintendo", new DateTime(2020, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Animal Crossing: New Horizons" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoGames");
        }
    }
}
