using EasyKanjiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EasyKanjiServer.Models.DTOs;

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

        [HttpPost]
        public async Task<ActionResult> GetToken(TokenDTO dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Username == dto.Username);

            if (user == null)
            {
                return NotFound(new { errors = "There is no such a user." });
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password + _configuration["AuthOptions:PEPPER"], user.PasswordHash))
            {
                return BadRequest(new { errors = "Password is incorrect." });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, dto.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var jwt = new JwtSecurityToken(
                issuer: _configuration["AuthOptions:ISSUER"],
                audience: _configuration["AuthOptions:AUDIENCE"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthOptions:KEY"]!)), SecurityAlgorithms.HmacSha256)
            );

            return new JsonResult(new { accessToken = new JwtSecurityTokenHandler().WriteToken(jwt), roles = new string[] { user.Role }, id = user.Id });
        }
    }
}
