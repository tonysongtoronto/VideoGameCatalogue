using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.Controllers;
using VideoGameCatalogue.Data;
using VideoGameCatalogue.Models;
using VideoGameCatalogue.Tests.TestData;
using Xunit;

namespace VideoGameCatalogue.Tests;

/// <summary>
/// 测试 VideoGamesController 的所有端点
/// </summary>
public class VideoGamesControllerTests
{
    // *** 修改 1: GetInMemoryDbContext 允许传入名称并控制是否填充数据 ***
    /// <summary>
    /// 为每个测试创建独立的内存数据库上下文，并可选择填充数据
    /// </summary>
    private GameDbContext GetInMemoryDbContext(string dbName, bool seedData = true)
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            // 使用传入的唯一名称
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        var context = new GameDbContext(options);

        // 如果需要，并且数据库是空的，则填充测试数据
        if (seedData && !context.VideoGames.Any())
        {
            context.VideoGames.AddRange(MockData.GetTestGames());
            context.SaveChanges();
        }

        return context;
    }

    #region GET Tests

    [Fact]
    public async Task GetVideoGames_ReturnsAllGames()
    {
        // *** 修改 2: 使用唯一的 DB 名称，并使用 using 确保释放 ***
        var dbName = Guid.NewGuid().ToString();
        using var context = GetInMemoryDbContext(dbName);
        var controller = new VideoGamesController(context);

        // Act - 执行
        var result = await controller.GetVideoGames();

        // Assert - 验证
        var actionResult = Assert.IsType<ActionResult<IEnumerable<VideoGame>>>(result);
        var games = Assert.IsAssignableFrom<IEnumerable<VideoGame>>(actionResult.Value);
        Assert.Equal(5, games.Count());
    }

    [Fact]
    public async Task GetVideoGames_ReturnsEmptyList_WhenNoGamesExist()
    {
        // *** 修改 2: 使用唯一的 DB 名称，不填充数据 ***
        var dbName = Guid.NewGuid().ToString();
        using var context = GetInMemoryDbContext(dbName, seedData: false); 
        var controller = new VideoGamesController(context);

        // Act
        var result = await controller.GetVideoGames();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<VideoGame>>>(result);
        var games = Assert.IsAssignableFrom<IEnumerable<VideoGame>>(actionResult.Value);
        Assert.Empty(games);
    }

    [Fact]
    public async Task GetVideoGame_WithValidId_ReturnsGame()
    {
        // *** 修改 2 ***
        var dbName = Guid.NewGuid().ToString();
        using var context = GetInMemoryDbContext(dbName);
        var controller = new VideoGamesController(context);

        // Act
        var result = await controller.GetVideoGame(1);

        // Assert
        var actionResult = Assert.IsType<ActionResult<VideoGame>>(result);
        var game = Assert.IsType<VideoGame>(actionResult.Value);
        Assert.Equal(1, game.Id);
        Assert.Equal("Test Game 1 - Action", game.Title);
    }

    [Fact]
    public async Task GetVideoGame_WithInvalidId_ReturnsNotFound()
    {
        // *** 修改 2 ***
        var dbName = Guid.NewGuid().ToString();
        using var context = GetInMemoryDbContext(dbName);
        var controller = new VideoGamesController(context);

        // Act
        var result = await controller.GetVideoGame(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetVideoGame_WithNegativeId_ReturnsNotFound()
    {
        // *** 修改 2 ***
        var dbName = Guid.NewGuid().ToString();
        using var context = GetInMemoryDbContext(dbName);
        var controller = new VideoGamesController(context);

        // Act
        var result = await controller.GetVideoGame(-1);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    #endregion

    #region POST Tests

    [Fact]
    public async Task CreateVideoGame_AddsNewGame_ReturnsCreatedGame()
    {
        var dbName = Guid.NewGuid().ToString();
        
        // Arrange
        using var context = GetInMemoryDbContext(dbName);
        var controller = new VideoGamesController(context);
        var newGame = MockData.GetNewGameForCreation();

        // Act
        var result = await controller.CreateVideoGame(newGame);

        // Assert 1
        var actionResult = Assert.IsType<ActionResult<VideoGame>>(result);
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        var game = Assert.IsType<VideoGame>(createdResult.Value);
        
        Assert.Equal("Brand New Game", game.Title);
        Assert.True(game.Id > 0); 
        
        // *** 修改 3 (Post 验证): 使用新的上下文验证持久化 ***
        // 验证数据库中确实添加了
        using var validationContext = GetInMemoryDbContext(dbName, seedData: false); 
        var dbGame = await validationContext.VideoGames.FindAsync(game.Id);
        Assert.NotNull(dbGame);
    }

    [Fact]
    public async Task CreateVideoGame_IncrementsGameCount()
    {
        var dbName = Guid.NewGuid().ToString();
        
        // Arrange
        using var initialContext = GetInMemoryDbContext(dbName);
        var controller = new VideoGamesController(initialContext);
        var initialCount = await initialContext.VideoGames.CountAsync();
        var newGame = MockData.GetNewGameForCreation();

        // Act
        await controller.CreateVideoGame(newGame);

        // Assert
        // *** 修改 3 (Post 验证): 使用新的上下文验证数量 ***
        using var validationContext = GetInMemoryDbContext(dbName, seedData: false); 
        var finalCount = await validationContext.VideoGames.CountAsync();
        Assert.Equal(initialCount + 1, finalCount);
    }

    #endregion

    #region PUT Tests

    // *** 核心修复 ***
    [Fact]
    public async Task UpdateVideoGame_WithValidData_UpdatesGame()
    {
        var dbName = Guid.NewGuid().ToString();

        // Arrange 1: 准备和填充数据 (使用 using 块确保 context 尽快被释放)
        using (var setupContext = GetInMemoryDbContext(dbName)) 
        {
            // setupContext 在这里被填充
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

        // Act: 执行更新操作
        // *** 关键修复: 使用一个单独的上下文实例来执行 Act ***
        using (var actContext = GetInMemoryDbContext(dbName, seedData: false)) 
        {
            var controller = new VideoGamesController(actContext);
            var result = await controller.UpdateVideoGame(1, updatedGame);

            // Assert 1: 验证返回状态码
            Assert.IsType<NoContentResult>(result);
        }
        // actContext 在这里被释放，所有跟踪对象被清除！

        // Assert 2: 验证更新成功
        // *** 关键修复: 使用另一个干净的上下文实例来验证数据是否已持久化 ***
        using (var validationContext = GetInMemoryDbContext(dbName, seedData: false))
        {
            var game = await validationContext.VideoGames.FindAsync(1);
            Assert.NotNull(game);
            Assert.Equal("Updated Title", game!.Title);
            Assert.Equal(79.99m, game.Price);
        }
    }

    [Fact]
    public async Task UpdateVideoGame_WithMismatchedId_ReturnsBadRequest()
    {
        // *** 修改 2 ***
        var dbName = Guid.NewGuid().ToString();
        using var context = GetInMemoryDbContext(dbName);
        var controller = new VideoGamesController(context);
        var updatedGame = MockData.GetUpdatedGame();
        updatedGame.Id = 1;

        // Act - ID 不匹配
        var result = await controller.UpdateVideoGame(2, updatedGame);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task UpdateVideoGame_WithNonExistentId_ReturnsNotFound()
    {
        // *** 修改 2 ***
        var dbName = Guid.NewGuid().ToString();
        using var context = GetInMemoryDbContext(dbName);
        var controller = new VideoGamesController(context);
        var updatedGame = new VideoGame
        {
            Id = 999,
            Title = "Non-existent Game",
            Publisher = "Publisher",
            Genre = "Genre",
            ReleaseDate = DateTime.Now,
            Price = 50.00m,
            Platform = "PC"
        };

        // Act
        var result = await controller.UpdateVideoGame(999, updatedGame);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region DELETE Tests

    [Fact]
    public async Task DeleteVideoGame_WithValidId_RemovesGame()
    {
        var dbName = Guid.NewGuid().ToString();
        
        // Arrange
        using var context = GetInMemoryDbContext(dbName);
        var controller = new VideoGamesController(context);
        var initialCount = await context.VideoGames.CountAsync();

        // Act
        var result = await controller.DeleteVideoGame(1);

        // Assert 1: 验证返回状态码
        Assert.IsType<NoContentResult>(result);
        
        // *** 修改 3 (Delete 验证): 使用新的上下文验证 ***
        using var validationContext = GetInMemoryDbContext(dbName, seedData: false); 
        
        // 验证删除成功
        var game = await validationContext.VideoGames.FindAsync(1);
        Assert.Null(game);
        
        // 验证数量减少 (使用新的上下文)
        var finalCount = await validationContext.VideoGames.CountAsync();
        Assert.Equal(initialCount - 1, finalCount);
    }

    [Fact]
    public async Task DeleteVideoGame_WithInvalidId_ReturnsNotFound()
    {
        // *** 修改 2 ***
        var dbName = Guid.NewGuid().ToString();
        using var context = GetInMemoryDbContext(dbName);
        var controller = new VideoGamesController(context);

        // Act
        var result = await controller.DeleteVideoGame(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteVideoGame_DoesNotAffectOtherGames()
    {
        var dbName = Guid.NewGuid().ToString();

        // Arrange & Act
        // *** 修改 3: 使用 using 块执行操作 ***
        using (var context = GetInMemoryDbContext(dbName))
        {
            var controller = new VideoGamesController(context);
            await controller.DeleteVideoGame(1);
        }

        // Assert: 使用新的上下文验证其他数据
        // *** 修改 3: 使用新的上下文验证 ***
        using var validationContext = GetInMemoryDbContext(dbName, seedData: false); 
        var game2 = await validationContext.VideoGames.FindAsync(2);
        var game3 = await validationContext.VideoGames.FindAsync(3);
        Assert.NotNull(game2);
        Assert.NotNull(game3);
    }

    #endregion

    #region Business Logic Tests

    [Fact]
    public async Task GetVideoGames_OrderedByTitle()
    {
        // *** 修改 2 ***
        var dbName = Guid.NewGuid().ToString();
        using var context = GetInMemoryDbContext(dbName);
        var controller = new VideoGamesController(context);

        // Act
        var result = await controller.GetVideoGames();
        var games = (result.Value as IEnumerable<VideoGame>)!.OrderBy(g => g.Title).ToList();

        // Assert
        for (int i = 0; i < games.Count - 1; i++)
        {
            Assert.True(string.Compare(games[i].Title, games[i + 1].Title, StringComparison.Ordinal) <= 0);
        }
    }

    [Fact]
    public async Task GetVideoGames_FilterByPriceRange()
    {
        // *** 修改 2 ***
        var dbName = Guid.NewGuid().ToString();
        using var context = GetInMemoryDbContext(dbName);

        // Act
        var affordableGames = await context.VideoGames
            .Where(g => g.Price >= 20 && g.Price <= 50)
            .ToListAsync();

        // Assert
        Assert.All(affordableGames, game => 
        {
            Assert.InRange(game.Price, 20, 50);
        });
    }

    #endregion
}