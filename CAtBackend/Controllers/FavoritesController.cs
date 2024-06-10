using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CatApp.Models;
using CatApp.Data;
using Microsoft.EntityFrameworkCore;

namespace CatApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FavoritesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddFavorite(Favorite favorite)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (favorite == null)
            {
                return BadRequest(new { message = "Favorite is null" });
            }

            favorite.UserId = int.Parse(userId);
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
            return Created("", favorite);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var favorites = await _context.Favorites.Where(f => f.UserId == userId).ToListAsync();
            return Ok(favorites);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> RemoveFavorite(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

            if (favorite == null)
            {
                return NotFound(new { message = "Favorite not found or not authorized" });
            }

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Favorite removed" });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateFavoriteName(int id, [FromBody] Favorite updatedFavorite)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (updatedFavorite == null)
            {
                return BadRequest(new { message = "Updated favorite is null" });
            }

            var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

            if (favorite == null)
            {
                return NotFound(new { message = "Favorite not found or not authorized" });
            }

            favorite.Name = updatedFavorite.Name;
            await _context.SaveChangesAsync();
            return Ok(favorite);
        }
    }
}
