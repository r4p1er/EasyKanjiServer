using EasyKanjiServer.Models;
using EasyKanjiServer.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EasyKanjiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly DBContext _db;
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;

        public UsersController(DBContext db, IConfiguration configuration, IDistributedCache cache)
        {
            _db = db;
            _configuration = configuration;
            _cache = cache;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IEnumerable<UserDTO>> GetUsers()
        {
            return await _db.Users.Include(x => x.Kanjis).Select(x => UserToDTO(x)).ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDTO>> GetUserById(int id)
        {
            var user = await _db.Users.Include(x => x.Kanjis).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return UserToDTO(user);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<UserDTO> GetMe()
        {
            return UserToDTO((await _db.Users.Include(x => x.Kanjis).FirstOrDefaultAsync(x => x.Username == User.FindFirstValue(ClaimTypes.Name)))!);
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> Register(UserPostDTO dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Username ==  dto.Username);

            if (user != null)
            {
                return BadRequest();
            }

            user = new User { Username = dto.Username, PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password + _configuration["AuthOptions:PEPPER"]), Role = "User" };
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
            Response.StatusCode = StatusCodes.Status201Created;

            return UserToDTO(user);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> EditCredentials(int id, UserPutDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var user = await _db.Users.Include(x => x.Kanjis).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            bool self = user.Username == User.FindFirstValue(ClaimTypes.Name);

            if (!self && User.IsInRole("User"))
            {
                return Forbid();
            }

            if (user.Username != dto.Username && !self)
            {
                return Forbid();
            }

            if (user.Username != dto.Username && await _db.Users.FirstOrDefaultAsync(x => x.Username == dto.Username) != null)
            {
                return BadRequest();
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password + _configuration["AuthOptions:PEPPER"], user.PasswordHash) && !self)
            {
                return Forbid();
            }

            if (user.Role != dto.Role && (User.IsInRole("User") || user.Username == "admin"))
            {
                return Forbid();
            }

            if (dto.Role != "User" && dto.Role != "Admin")
            {
                return BadRequest();
            }

            user.Username = dto.Username;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password + _configuration["AuthOptions:PEPPER"]);
            user.Role = dto.Role;
            await _db.SaveChangesAsync();

            string header = HttpContext.Request.Headers.ContainsKey(HeaderNames.Authorization) ? HttpContext.Request.Headers[HeaderNames.Authorization].ToString() : string.Empty;
            if (!string.IsNullOrEmpty(header) && header.StartsWith("Bearer"))
            {
                string encodedJwt = header.Split(" ")[1];
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(encodedJwt);
                await _cache.SetStringAsync(encodedJwt, "invalid", new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = token.ValidTo
                });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var user = await _db.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if ((user.Username != User.FindFirstValue(ClaimTypes.Name) && User.IsInRole("User")) || user.Username == "admin")
            {
                return Forbid();
            }

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private static UserDTO UserToDTO(User user)
        {
            return new UserDTO { Id = user.Id, Username = user.Username, Role = user.Role, Kanjis = user.Kanjis };
        }
    }
}
