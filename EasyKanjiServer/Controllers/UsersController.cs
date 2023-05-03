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
                return NotFound(new { errors = "There is no such a user." });
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
                return BadRequest(new { errors = "Such a user already exists." });
            }

            user = new User { Username = dto.Username, PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password + _configuration["AuthOptions:PEPPER"]), Role = "User" };
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
            Response.StatusCode = StatusCodes.Status201Created;

            return UserToDTO(user);
        }

        [HttpPatch("{id:min(1)}")]
        [Authorize]
        public async Task<ActionResult> EditCredentials(int id, UserPatchCredentialsDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) && string.IsNullOrWhiteSpace(dto.Password) && string.IsNullOrWhiteSpace(dto.Role))
            {
                return BadRequest(new { errors = "You didn't provide any changes." });
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.PasswordCheck + _configuration["AuthOptions:PEPPER"], (await _db.Users.FirstOrDefaultAsync(x => x.Username == User.FindFirstValue(ClaimTypes.Name)))!.PasswordHash))
            {
                return BadRequest(new { errors = "Your password is incorrect." });
            }

            var user = await _db.Users.Include(x => x.Kanjis).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound(new { errors = "There is no such a user." });
            }

            bool self = user.Username == User.FindFirstValue(ClaimTypes.Name);

            if (!self && User.IsInRole("User"))
            {
                return Forbid();
            }

            if (!string.IsNullOrWhiteSpace(dto.Username) && !self)
            {
                return Forbid();
            }

            if (!string.IsNullOrWhiteSpace(dto.Username) && await _db.Users.FirstOrDefaultAsync(x => x.Username == dto.Username) != null)
            {
                return BadRequest(new { errors = "This username is already taken." });
            }

            if (!string.IsNullOrWhiteSpace(dto.Username) && (dto.Username.Length < 2 || dto.Username.Length > 16))
            {
                return BadRequest(new { errors = "Username length should be from 2 to 16." });
            }

            if (!string.IsNullOrWhiteSpace(dto.Password) && !self)
            {
                return Forbid();
            }

            if (!string.IsNullOrWhiteSpace(dto.Password) && (dto.Password.Length < 6 || dto.Password.Length > 24))
            {
                return BadRequest(new { errors = "Password length should be from 6 to 24." });
            }

            if (!string.IsNullOrWhiteSpace(dto.Role) && (User.IsInRole("User") || user.Username == "admin"))
            {
                return Forbid();
            }

            if (!string.IsNullOrWhiteSpace(dto.Role) && dto.Role != "User" && dto.Role != "Admin")
            {
                return BadRequest(new { errors = "There is no such a role." });
            }

            user.Username = !string.IsNullOrWhiteSpace(dto.Username) ? dto.Username : user.Username;
            user.PasswordHash = !string.IsNullOrWhiteSpace(dto.Password) ? BCrypt.Net.BCrypt.HashPassword(dto.Password + _configuration["AuthOptions:PEPPER"]) : user.PasswordHash;
            user.Role = !string.IsNullOrWhiteSpace(dto.Role) ? dto.Role : user.Role;
            await _db.SaveChangesAsync();
            await _cache.SetStringAsync(user.Username, "invalid");

            return NoContent();
        }

        [HttpPatch("kanjis/{method:alpha}")]
        [Authorize]
        public async Task<ActionResult> AddKanjis(string method, int[] ids)
        {
            if (method != "add" && method != "remove")
            {
                return BadRequest(new {errors = "Action should be either add or remove."});
            }

            var user = await _db.Users.Include(x => x.Kanjis).FirstOrDefaultAsync(x => x.Username == User.FindFirstValue(ClaimTypes.Name));
            
            foreach (var id in ids)
            {
                var kanji = user!.Kanjis.FirstOrDefault(x => x.Id == id);

                if (kanji == null && method == "add")
                {
                    var dbKanji = await _db.Kanjis.FindAsync(id);

                    if (dbKanji == null)
                    {
                        return NotFound(new { errors = "There is no such a kanji." });
                    }

                    user.Kanjis.Add(dbKanji);
                }

                if (kanji != null && method == "remove")
                {
                    user.Kanjis.Remove(kanji);
                }
            }

            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var user = await _db.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { errors = "There is no such a user." });
            }

            if ((user.Username != User.FindFirstValue(ClaimTypes.Name) && User.IsInRole("User")) || user.Username == "admin")
            {
                return Forbid();
            }

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            await _cache.SetStringAsync(user.Username, "invalid");

            return NoContent();
        }

        private static UserDTO UserToDTO(User user)
        {
            return new UserDTO { Id = user.Id, Username = user.Username, Role = user.Role, Kanjis = KanjisToKanjiDTOs(user.Kanjis) };
        }

        private static List<KanjiDTO> KanjisToKanjiDTOs(List<Kanji> kanjis)
        {
            return kanjis.Select(x => new KanjiDTO { Id = x.Id, KunReadings = x.KunReadings, OnReadings = x.OnReadings, Meaning = x.Meaning, Writing = x.Writing }).ToList();
        }
    }
}
