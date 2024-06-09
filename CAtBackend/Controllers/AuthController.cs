using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CatApp.Models;
using CatApp.Services;
using Microsoft.AspNetCore.Cors;

namespace CatApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowAll")]
    public class AuthController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;
        private readonly IConfiguration _config;

        public AuthController(MongoDBService mongoDBService, IConfiguration config)
        {
            _mongoDBService = mongoDBService;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            if (user == null)
            {
                return BadRequest(new { message = "User is null" });
            }

            user.HashPassword();
            try
            {
                await _mongoDBService.Users.InsertOneAsync(user);
                var token = GenerateJwtToken(user);
                return Created("", new { token });
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                return BadRequest(new { message = "Username already exists" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest(new { message = "User is null" });
            }

            var dbUser = await _mongoDBService.Users.Find(u => u.Username == user.Username).FirstOrDefaultAsync();

            if (dbUser == null || !dbUser.VerifyPassword(user.Password))
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var token = GenerateJwtToken(dbUser);
            return Ok(new { token });
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddHours(1),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
