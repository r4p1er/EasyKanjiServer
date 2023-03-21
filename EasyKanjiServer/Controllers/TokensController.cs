using EasyKanjiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EasyKanjiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokensController : ControllerBase
    {
        private readonly DBContext _db;
        private readonly IConfiguration _configuration;

        public TokensController(DBContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(string username, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user == null)
            {
                return NotFound();
            }

            if (!BCrypt.Net.BCrypt.Verify(password + _configuration["AuthOptions:PEPPER"], user.PasswordHash))
            {
                return BadRequest();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var jwt = new JwtSecurityToken(
                issuer: _configuration["AuthOptions:ISSUER"],
                audience: _configuration["AuthOptions:AUDIENCE"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthOptions:KEY"]!)), SecurityAlgorithms.HmacSha256)
            );
            return new JsonResult(new JwtSecurityTokenHandler().WriteToken(jwt));
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(string username, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user != null)
            {
                return BadRequest();
            }

            user = new User { Username = username, PasswordHash = BCrypt.Net.BCrypt.HashPassword(password + _configuration["AuthOptions:PEPPER"]), Role = "User" };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
