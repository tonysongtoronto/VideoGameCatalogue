using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.Data;
using VideoGameCatalogue.Models;
using VideoGameCatalogue.Tests.TestData;
using Xunit;

namespace VideoGameCatalogue.Tests;

/// <summary>
/// 测试 GameDbContext 的功能
/// </summary>
public class GameDbContextTests
{
    // *** 修改 1: GetInMemoryDbContext() 只创建空的上下文 ***
    /// <summary>
    /// 创建内存数据库上下文
    /// </summary>
    private GameDbContext GetInMemoryDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            // 使用传入的唯一名称
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new GameDbContext(options);
    }

    [Fact]
    public async Task CanAddGameToDatabase()
    {
        var dbName = Guid.NewGuid().ToString();
        // Arrange
        using var context = GetInMemoryDbContext(dbName);
        var game = MockData.GetSingleTestGame();

        // Act
        context.VideoGames.Add(game);
        await context.SaveChangesAsync();
        // context 在这里被释放

        // Assert: 使用新的上下文验证持久化
        using var validationContext = GetInMemoryDbContext(dbName);
        var savedGame = await validationContext.VideoGames.FindAsync(game.Id);
        Assert.NotNull(savedGame);
        Assert.Equal(game.Title, savedGame.Title);
    }

    [Fact]
    public async Task CanRetrieveAllGames()
    {
        var dbName = Guid.NewGuid().ToString();
        // Arrange: 填充数据
        using (var setupContext = GetInMemoryDbContext(dbName))
        {
            var games = MockData.GetTestGames();
            setupContext.VideoGames.AddRange(games);
            await setupContext.SaveChangesAsync();
        }
        
        // Act & Assert: 使用新的上下文读取
        using (var context = GetInMemoryDbContext(dbName))
        {
            // Act
            var retrievedGames = await context.VideoGames.ToListAsync();

            // Assert
            Assert.Equal(MockData.GetTestGames().Count, retrievedGames.Count);
        }
    }

    [Fact]
    public async Task CanUpdateGame()
    {
        var dbName = Guid.NewGuid().ToString();
        var gameToUpdate = MockData.GetSingleTestGame();
        
        // Arrange: 填充初始数据
        using (var setupContext = GetInMemoryDbContext(dbName))
        {
            setupContext.VideoGames.Add(gameToUpdate);
            await setupContext.SaveChangesAsync();
        }

        // Act: 执行更新操作
        using (var updateContext = GetInMemoryDbContext(dbName))
        {
            // Attach 现有的跟踪对象（这里我们从 Mock 中再次创建，并手动设置 ID）
            var game = await updateContext.VideoGames.FindAsync(gameToUpdate.Id);
            Assert.NotNull(game);
            
            game.Price = 99.99m;
            // updateContext.VideoGames.Update(game); // Find/Attach 后会自动跟踪
            await updateContext.SaveChangesAsync();
        }
        
        // Assert: 使用新的上下文验证更新结果
        using (var validationContext = GetInMemoryDbContext(dbName))
        {
            var updatedGame = await validationContext.VideoGames.FindAsync(gameToUpdate.Id);
            Assert.Equal(99.99m, updatedGame!.Price);
        }
    }

    [Fact]
    public async Task CanDeleteGame()
    {
        var dbName = Guid.NewGuid().ToString();
        var gameToDelete = MockData.GetSingleTestGame();
        
        // Arrange: 填充初始数据
        using (var setupContext = GetInMemoryDbContext(dbName))
        {
            setupContext.VideoGames.Add(gameToDelete);
            await setupContext.SaveChangesAsync();
        }
        
        // Act: 执行删除操作
        using (var deleteContext = GetInMemoryDbContext(dbName))
        {
            var game = await deleteContext.VideoGames.FindAsync(gameToDelete.Id);
            Assert.NotNull(game);

            deleteContext.VideoGames.Remove(game);
            await deleteContext.SaveChangesAsync();
        }
        
        // Assert: 使用新的上下文验证删除结果
        using (var validationContext = GetInMemoryDbContext(dbName))
        {
            var deletedGame = await validationContext.VideoGames.FindAsync(gameToDelete.Id);
            Assert.Null(deletedGame);
        }
    }

    [Fact]
    public async Task CanFilterGamesByGenre()
    {
        var dbName = Guid.NewGuid().ToString();
        // Arrange: 填充数据
        using (var setupContext = GetInMemoryDbContext(dbName))
        {
            setupContext.VideoGames.AddRange(MockData.GetTestGames());
            await setupContext.SaveChangesAsync();
        }

        // Act & Assert
        using (var context = GetInMemoryDbContext(dbName))
        {
            // Act
            var actionGames = await context.VideoGames
                .Where(g => g.Genre == "Action")
                .ToListAsync();

            // Assert
            Assert.Single(actionGames);
            Assert.Equal("Action", actionGames[0].Genre);
        }
    }

    [Fact]
    public async Task CanSortGamesByPrice()
    {
        var dbName = Guid.NewGuid().ToString();
        // Arrange: 填充数据
        using (var setupContext = GetInMemoryDbContext(dbName))
        {
            setupContext.VideoGames.AddRange(MockData.GetTestGames());
            await setupContext.SaveChangesAsync();
        }

        // Act & Assert
        using (var context = GetInMemoryDbContext(dbName))
        {
            // Act
            var sortedGames = await context.VideoGames
                .OrderBy(g => g.Price)
                .ToListAsync();

            // Assert
            Assert.Equal(19.99m, sortedGames.First().Price);
            Assert.Equal(59.99m, sortedGames.Last().Price);
        }
    }
}