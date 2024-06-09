using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;
using CatApp.Models;
using CatApp.Services;

namespace CatApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;

        public FavoritesController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
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

            favorite.UserId = userId;
            await _mongoDBService.Favorites.InsertOneAsync(favorite);
            return Created("", favorite);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var favorites = await _mongoDBService.Favorites.Find(f => f.UserId == userId).ToListAsync();
            return Ok(favorites);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> RemoveFavorite(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _mongoDBService.Favorites.DeleteOneAsync(f => f.Id == id && f.UserId == userId);

            if (result.DeletedCount == 0)
            {
                return NotFound(new { message = "Favorite not found or not authorized" });
            }

            return Ok(new { message = "Favorite removed" });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateFavoriteName(string id, [FromBody] Favorite updatedFavorite)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (updatedFavorite == null)
            {
                return BadRequest(new { message = "Updated favorite is null" });
            }

            var favorite = await _mongoDBService.Favorites.Find(f => f.Id == id && f.UserId == userId).FirstOrDefaultAsync();

            if (favorite == null)
            {
                return NotFound(new { message = "Favorite not found or not authorized" });
            }

            favorite.Name = updatedFavorite.Name;
            await _mongoDBService.Favorites.ReplaceOneAsync(f => f.Id == id, favorite);
            return Ok(favorite);
        }
    }
}
