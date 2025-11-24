namespace VideoGameCatalogue.Data;

using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.Models;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
    {
    }

    public DbSet<VideoGame> VideoGames { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<VideoGame>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Publisher).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Genre).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Platform).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
        });

        // Seed data - 15条模拟数据
        modelBuilder.Entity<VideoGame>().HasData(
            new VideoGame { Id = 1, Title = "The Legend of Zelda: Breath of the Wild", Publisher = "Nintendo", Genre = "Action-Adventure", ReleaseDate = new DateTime(2017, 3, 3), Price = 59.99m, Platform = "Nintendo Switch" },
            new VideoGame { Id = 2, Title = "Elden Ring", Publisher = "FromSoftware", Genre = "Action RPG", ReleaseDate = new DateTime(2022, 2, 25), Price = 59.99m, Platform = "PC" },
            new VideoGame { Id = 3, Title = "God of War", Publisher = "Sony Interactive Entertainment", Genre = "Action-Adventure", ReleaseDate = new DateTime(2018, 4, 20), Price = 49.99m, Platform = "PlayStation 5" },
            new VideoGame { Id = 4, Title = "Cyberpunk 2077", Publisher = "CD Projekt Red", Genre = "RPG", ReleaseDate = new DateTime(2020, 12, 10), Price = 39.99m, Platform = "PC" },
            new VideoGame { Id = 5, Title = "Hades", Publisher = "Supergiant Games", Genre = "Roguelike", ReleaseDate = new DateTime(2020, 9, 17), Price = 24.99m, Platform = "Nintendo Switch" },
            new VideoGame { Id = 6, Title = "Red Dead Redemption 2", Publisher = "Rockstar Games", Genre = "Action-Adventure", ReleaseDate = new DateTime(2018, 10, 26), Price = 59.99m, Platform = "Xbox Series X" },
            new VideoGame { Id = 7, Title = "Stardew Valley", Publisher = "ConcernedApe", Genre = "Simulation", ReleaseDate = new DateTime(2016, 2, 26), Price = 14.99m, Platform = "PC" },
            new VideoGame { Id = 8, Title = "Hollow Knight", Publisher = "Team Cherry", Genre = "Metroidvania", ReleaseDate = new DateTime(2017, 2, 24), Price = 14.99m, Platform = "PC" },
            new VideoGame { Id = 9, Title = "The Witcher 3: Wild Hunt", Publisher = "CD Projekt Red", Genre = "RPG", ReleaseDate = new DateTime(2015, 5, 19), Price = 39.99m, Platform = "PC" },
            new VideoGame { Id = 10, Title = "Minecraft", Publisher = "Mojang Studios", Genre = "Sandbox", ReleaseDate = new DateTime(2011, 11, 18), Price = 26.95m, Platform = "PC" },
            new VideoGame { Id = 11, Title = "Dark Souls III", Publisher = "FromSoftware", Genre = "Action RPG", ReleaseDate = new DateTime(2016, 4, 12), Price = 39.99m, Platform = "PlayStation 4" },
            new VideoGame { Id = 12, Title = "Celeste", Publisher = "Maddy Makes Games", Genre = "Platformer", ReleaseDate = new DateTime(2018, 1, 25), Price = 19.99m, Platform = "Nintendo Switch" },
            new VideoGame { Id = 13, Title = "Portal 2", Publisher = "Valve", Genre = "Puzzle", ReleaseDate = new DateTime(2011, 4, 19), Price = 9.99m, Platform = "PC" },
            new VideoGame { Id = 14, Title = "Sekiro: Shadows Die Twice", Publisher = "FromSoftware", Genre = "Action-Adventure", ReleaseDate = new DateTime(2019, 3, 22), Price = 59.99m, Platform = "PC" },
            new VideoGame { Id = 15, Title = "Animal Crossing: New Horizons", Publisher = "Nintendo", Genre = "Simulation", ReleaseDate = new DateTime(2020, 3, 20), Price = 59.99m, Platform = "Nintendo Switch" }
        );
    }
}