namespace VideoGameCatalogue.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameCatalogue.Data;
using VideoGameCatalogue.Models;

[ApiController]
[Route("api/[controller]")]
public class VideoGamesController : ControllerBase
{
    private readonly GameDbContext _context;

    public VideoGamesController(GameDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VideoGame>>> GetVideoGames()
    {
        return await _context.VideoGames.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VideoGame>> GetVideoGame(int id)
    {
        var game = await _context.VideoGames.FindAsync(id);
        if (game == null) return NotFound();
        return game;
    }

    [HttpPost]
    public async Task<ActionResult<VideoGame>> CreateVideoGame(VideoGame game)
    {
        _context.VideoGames.Add(game);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetVideoGame), new { id = game.Id }, game);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVideoGame(int id, VideoGame game)
    {
        if (id != game.Id) return BadRequest();
        _context.Entry(game).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await VideoGameExists(id)) return NotFound();
            throw;
        }
       ? return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVideoGame(int id)
    {
        var game = await _context.VideoGames.FindAsync(id);
        if (game == null) return NotFound();
        _context.VideoGames.Remove(game);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<bool> VideoGameExists(int id)
    {
        return await _context.VideoGames.AnyAsync(e => e.Id == id);
    }
}