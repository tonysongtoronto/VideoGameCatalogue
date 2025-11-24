using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.Controllers;
using VideoGameCatalogue.Data;
using VideoGameCatalogue.Models;
using VideoGameCatalogue.Tests.TestData;
using Xunit;

namespace VideoGameCatalogue.Tests;

/// <summary>
/// Tests for VideoGamesController endpoints using isolated in-memory databases.
/// </summary>
public class VideoGamesControllerTests
{
    /// <summary>
    /// Creates a fresh GameDbContext with an isolated in-memory database.
    /// Caller is responsible for seeding data if needed.
    /// </summary>
    private static GameDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new GameDbContext(options);
    }

    /// <summary>
    /// Seeds the context with test data and saves changes.
    /// </summary>
    private static void SeedTestData(GameDbContext context)
    {
        if (!context.VideoGames.Any())
        {
            context.VideoGames.AddRange(MockData.GetTestGames());
            context.SaveChanges();
        }
    }

    #region GET Tests

    [Fact]
    public async Task GetVideoGames_ReturnsAllGames()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateContext(dbName);
        SeedTestData(context);
        var controller = new VideoGamesController(context);

        var result = await controller.GetVideoGames();

        var actionResult = Assert.IsType<ActionResult<IEnumerable<VideoGame>>>(result);
        var games = Assert.IsAssignableFrom<IEnumerable<VideoGame>>(actionResult.Value);
        Assert.Equal(5, games.Count());
    }

    [Fact]
    public async Task GetVideoGames_ReturnsEmptyList_WhenNoGamesExist()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateContext(dbName); // No seed
        var controller = new VideoGamesController(context);

        var result = await controller.GetVideoGames();

        var actionResult = Assert.IsType<ActionResult<IEnumerable<VideoGame>>>(result);
        var games = Assert.IsAssignableFrom<IEnumerable<VideoGame>>(actionResult.Value);
        Assert.Empty(games);
    }

    [Fact]
    public async Task GetVideoGame_WithValidId_ReturnsGame()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateContext(dbName);
        SeedTestData(context);
        var controller = new VideoGamesController(context);

        var result = await controller.GetVideoGame(1);

        var actionResult = Assert.IsType<ActionResult<VideoGame>>(result);
        var game = Assert.IsType<VideoGame>(actionResult.Value);
        Assert.Equal(1, game.Id);
        Assert.Equal("Test Game 1 - Action", game.Title);
    }

    [Fact]
    public async Task GetVideoGame_WithInvalidId_ReturnsNotFound()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateContext(dbName);
        SeedTestData(context);
        var controller = new VideoGamesController(context);

        var result = await controller.GetVideoGame(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetVideoGame_WithNegativeId_ReturnsNotFound()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateContext(dbName);
        SeedTestData(context);
        var controller = new VideoGamesController(context);

        var result = await controller.GetVideoGame(-1);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetVideoGames_ReturnsGamesOrderedByTitle()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateContext(dbName);
        SeedTestData(context);
        var controller = new VideoGamesController(context);

        var result = await controller.GetVideoGames();

        var games = Assert.IsAssignableFrom<IEnumerable<VideoGame>>(result.Value);
        var gameList = games.ToList();

        // Verify the list is already sorted by Title (as returned by controller)
        for (int i = 0; i < gameList.Count - 1; i++)
        {
            Assert.True(
                string.Compare(gameList[i].Title, gameList[i + 1].Title, StringComparison.Ordinal) <= 0,
                $"Game at index {i} ('{gameList[i].Title}') should come before index {i + 1} ('{gameList[i + 1].Title}')"
            );
        }
    }

    #endregion

    #region POST Tests

    [Fact]
    public async Task CreateVideoGame_AddsNewGame_ReturnsCreatedGame()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateContext(dbName);
        SeedTestData(context);
        var controller = new VideoGamesController(context);
        var newGame = MockData.GetNewGameForCreation();

        var result = await controller.CreateVideoGame(newGame);

        var actionResult = Assert.IsType<ActionResult<VideoGame>>(result);
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        var game = Assert.IsType<VideoGame>(createdResult.Value);

        Assert.Equal("Brand New Game", game.Title);
        Assert.True(game.Id > 0);

        // Validate persistence with a fresh context
        using var validationContext = CreateContext(dbName);
        var dbGame = await validationContext.VideoGames.FindAsync(game.Id);
        Assert.NotNull(dbGame);
    }

    [Fact]
    public async Task CreateVideoGame_IncrementsGameCount()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateContext(dbName);
        SeedTestData(context);
        var initialCount = await context.VideoGames.CountAsync();

        var controller = new VideoGamesController(context);
        await controller.CreateVideoGame(MockData.GetNewGameForCreation());

        using var validationContext = CreateContext(dbName);
        var finalCount = await validationContext.VideoGames.CountAsync();
        Assert.Equal(initialCount + 1, finalCount);
    }

    #endregion

    #region PUT Tests

    [Fact]
    public async Task UpdateVideoGame_WithValidData_UpdatesGame()
    {
        var dbName = Guid.NewGuid().ToString();

        // Arrange: Seed data
        using (var setupContext = CreateContext(dbName))
        {
            SeedTestData(setupContext);
        }

        var updatedGame = new VideoGame
        {
            Id = 1,
            Title = "Updated Title",
            Publisher = "Updated Publisher",
            Genre = "Updated Genre",
            ReleaseDate = new DateTime(2023, 12, 31),
            Price = 79.99m,
            Platform = "Updated Platform"
        };

        // Act
        using (var actContext = CreateContext(dbName))
        {
            var controller = new VideoGamesController(actContext);
            var result = await controller.UpdateVideoGame(1, updatedGame);
            Assert.IsType<NoContentResult>(result);
        }

        // Assert: Validate with fresh context
        using (var validationContext = CreateContext(dbName))
        {
            var game = await validationContext.VideoGames.FindAsync(1);
            Assert.NotNull(game);
            Assert.Equal("Updated Title", game.Title);
            Assert.Equal(79.99m, game.Price);
        }
    }

    [Fact]
    public async Task UpdateVideoGame_WithMismatchedId_ReturnsBadRequest()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateContext(dbName);
        SeedTestData(context);
        var controller = new VideoGamesController(context);
        var updatedGame = MockData.GetUpdatedGame();
        updatedGame.Id = 1;

        var result = await controller.UpdateVideoGame(2, updatedGame);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task UpdateVideoGame_WithNonExistentId_ReturnsNotFound()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateContext(dbName);
        SeedTestData(context);
        var controller = new VideoGamesController(context);
        var updatedGame = new VideoGame { Id = 999, Title = "Ghost" };

        var result = await controller.UpdateVideoGame(999, updatedGame);

        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region DELETE Tests

    [Fact]
    public async Task DeleteVideoGame_WithValidId_RemovesGame()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateContext(dbName);
        SeedTestData(context);
        var initialCount = await context.VideoGames.CountAsync();

        var controller = new VideoGamesController(context);
        var result = await controller.DeleteVideoGame(1);

        Assert.IsType<NoContentResult>(result);

        using var validationContext = CreateContext(dbName);
        var game = await validationContext.VideoGames.FindAsync(1);
        Assert.Null(game);

        var finalCount = await validationContext.VideoGames.CountAsync();
        Assert.Equal(initialCount - 1, finalCount);
    }

    [Fact]
    public async Task DeleteVideoGame_WithInvalidId_ReturnsNotFound()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateContext(dbName);
        SeedTestData(context);
        var controller = new VideoGamesController(context);

        var result = await controller.DeleteVideoGame(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteVideoGame_DoesNotAffectOtherGames()
    {
        var dbName = Guid.NewGuid().ToString();
        using (var context = CreateContext(dbName))
        {
            SeedTestData(context);
            var controller = new VideoGamesController(context);
            await controller.DeleteVideoGame(1);
        }

        using var validationContext = CreateContext(dbName);
        var game2 = await validationContext.VideoGames.FindAsync(2);
        var game3 = await validationContext.VideoGames.FindAsync(3);
        Assert.NotNull(game2);
        Assert.NotNull(game3);
    }

    #endregion
}