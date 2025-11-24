using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.Data;
using VideoGameCatalogue.Models;
using VideoGameCatalogue.Tests.TestData;
using Xunit;

namespace VideoGameCatalogue.Tests;

/// <summary>
/// Tests for GameDbContext functionality using in-memory database.
/// </summary>
public class GameDbContextTests
{
    private GameDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new GameDbContext(options);
    }

    [Fact]
    public async Task CanAddGameToDatabase()
    {
        var dbName = Guid.NewGuid().ToString();
        var game = MockData.GetSingleTestGame();

        // Act
        using (var context = CreateContext(dbName))
        {
            context.VideoGames.Add(game);
            await context.SaveChangesAsync();
        }

        // Assert
        using (var context = CreateContext(dbName))
        {
            var savedGame = await context.VideoGames.FindAsync(game.Id);
            Assert.NotNull(savedGame);
            Assert.Equal(game.Title, savedGame.Title);
        }
    }

    [Fact]
    public async Task CanRetrieveAllGames()
    {
        var dbName = Guid.NewGuid().ToString();
        var expectedCount = MockData.GetTestGames().Count;

        // Arrange
        using (var context = CreateContext(dbName))
        {
            context.VideoGames.AddRange(MockData.GetTestGames());
            await context.SaveChangesAsync();
        }

        // Act & Assert
        using (var context = CreateContext(dbName))
        {
            var retrievedGames = await context.VideoGames.ToListAsync();
            Assert.Equal(expectedCount, retrievedGames.Count);
        }
    }

    [Fact]
    public async Task CanUpdateGame()
    {
        var dbName = Guid.NewGuid().ToString();
        var originalGame = MockData.GetSingleTestGame();
        const decimal updatedPrice = 99.99m;

        // Arrange
        using (var context = CreateContext(dbName))
        {
            context.VideoGames.Add(originalGame);
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = CreateContext(dbName))
        {
            var game = await context.VideoGames.FindAsync(originalGame.Id);
            Assert.NotNull(game);

            game.Price = updatedPrice;
            await context.SaveChangesAsync();
        }

        // Assert
        using (var context = CreateContext(dbName))
        {
            var updatedGame = await context.VideoGames.FindAsync(originalGame.Id);
            Assert.NotNull(updatedGame);
            Assert.Equal(updatedPrice, updatedGame.Price);
        }
    }

    [Fact]
    public async Task CanDeleteGame()
    {
        var dbName = Guid.NewGuid().ToString();
        var gameToDelete = MockData.GetSingleTestGame();

        // Arrange
        using (var context = CreateContext(dbName))
        {
            context.VideoGames.Add(gameToDelete);
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = CreateContext(dbName))
        {
            var game = await context.VideoGames.FindAsync(gameToDelete.Id);
            Assert.NotNull(game);

            context.VideoGames.Remove(game);
            await context.SaveChangesAsync();
        }

        // Assert
        using (var context = CreateContext(dbName))
        {
            var deletedGame = await context.VideoGames.FindAsync(gameToDelete.Id);
            Assert.Null(deletedGame);
        }
    }

    [Fact]
    public async Task CanFilterGamesByGenre()
    {
        var dbName = Guid.NewGuid().ToString();
        const string targetGenre = "Action";

        // Arrange
        using (var context = CreateContext(dbName))
        {
            context.VideoGames.AddRange(MockData.GetTestGames());
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = CreateContext(dbName))
        {
            var actionGames = await context.VideoGames
                .Where(g => g.Genre == targetGenre)
                .ToListAsync();

            // Assert
            Assert.Single(actionGames);
            Assert.Equal(targetGenre, actionGames[0].Genre);
        }
    }

    [Fact]
    public async Task CanSortGamesByPrice()
    {
        var dbName = Guid.NewGuid().ToString();
        const decimal expectedMinPrice = 19.99m;
        const decimal expectedMaxPrice = 59.99m;

        // Arrange
        using (var context = CreateContext(dbName))
        {
            context.VideoGames.AddRange(MockData.GetTestGames());
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = CreateContext(dbName))
        {
            var sortedGames = await context.VideoGames
                .OrderBy(g => g.Price)
                .ToListAsync();

            // Assert
            Assert.Equal(expectedMinPrice, sortedGames.First().Price);
            Assert.Equal(expectedMaxPrice, sortedGames.Last().Price);
        }
    }
}