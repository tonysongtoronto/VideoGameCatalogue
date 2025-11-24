using VideoGameCatalogue.Models;

namespace VideoGameCatalogue.Tests.TestData;

/// <summary>
/// Provides mock data for testing
/// </summary>
public static class MockData
{
    /// <summary>
    /// Returns a list of test video games
    /// </summary>
    public static List<VideoGame> GetTestGames()
    {
        return new List<VideoGame>
        {
            new VideoGame
            {
                Id = 1,
                Title = "Test Game 1 - Action",
                Publisher = "Test Publisher A",
                Genre = "Action",
                ReleaseDate = new DateTime(2020, 1, 15),
                Price = 49.99m,
                Platform = "PC"
            },
            new VideoGame
            {
                Id = 2,
                Title = "Test Game 2 - RPG",
                Publisher = "Test Publisher B",
                Genre = "RPG",
                ReleaseDate = new DateTime(2021, 6, 20),
                Price = 59.99m,
                Platform = "PlayStation 5"
            },
            new VideoGame
            {
                Id = 3,
                Title = "Test Game 3 - Strategy",
                Publisher = "Test Publisher C",
                Genre = "Strategy",
                ReleaseDate = new DateTime(2019, 3, 10),
                Price = 39.99m,
                Platform = "Xbox Series X"
            },
            new VideoGame
            {
                Id = 4,
                Title = "Test Game 4 - Adventure",
                Publisher = "Test Publisher D",
                Genre = "Adventure",
                ReleaseDate = new DateTime(2022, 11, 5),
                Price = 29.99m,
                Platform = "Nintendo Switch"
            },
            new VideoGame
            {
                Id = 5,
                Title = "Test Game 5 - Simulation",
                Publisher = "Test Publisher E",
                Genre = "Simulation",
                ReleaseDate = new DateTime(2018, 8, 12),
                Price = 19.99m,
                Platform = "PC"
            }
        };
    }

    /// <summary>
    /// Returns a single test video game
    /// </summary>
    public static VideoGame GetSingleTestGame()
    {
        return new VideoGame
        {
            Id = 1,
            Title = "Single Test Game",
            Publisher = "Test Publisher",
            Genre = "Action",
            ReleaseDate = new DateTime(2023, 1, 1),
            Price = 59.99m,
            Platform = "PC"
        };
    }

    /// <summary>
    /// Returns a new video game instance for creation (without an ID)
    /// </summary>
    public static VideoGame GetNewGameForCreation()
    {
        return new VideoGame
        {
            Title = "Brand New Game",
            Publisher = "New Publisher",
            Genre = "Indie",
            ReleaseDate = new DateTime(2024, 5, 15),
            Price = 24.99m,
            Platform = "PC"
        };
    }

    /// <summary>
    /// Returns an updated video game instance (with existing ID)
    /// </summary>
    public static VideoGame GetUpdatedGame()
    {
        return new VideoGame
        {
            Id = 1,
            Title = "Updated Game Title",
            Publisher = "Updated Publisher",
            Genre = "Updated Genre",
            ReleaseDate = new DateTime(2023, 12, 31),
            Price = 69.99m,
            Platform = "Multi-Platform"
        };
    }

    /// <summary>
    /// Returns an invalid video game instance (for validation tests)
    /// </summary>
    public static VideoGame GetInvalidGame()
    {
        return new VideoGame
        {
            Id = -1,
            Title = "", // Empty title
            Publisher = "",
            Genre = "",
            ReleaseDate = DateTime.MinValue,
            Price = -10.00m, // Negative price
            Platform = ""
        };
    }
}