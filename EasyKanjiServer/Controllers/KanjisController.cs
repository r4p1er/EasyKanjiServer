using EasyKanjiServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasyKanjiServer.Models.DTOs;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace EasyKanjiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KanjisController : ControllerBase
    {
        private readonly DBContext _db;

        public KanjisController(DBContext db)
        {
            _db = db;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<KanjiDTO>> GetKanji(int id)
        {
            var kanji = await _db.Kanjis.FindAsync(id);

            if (kanji == null)
            {
                return NotFound();
            }
            return KanjiToDTO(kanji);

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<KanjiDTO>>> GetKanjis(int[] ids)
        {
            var kanjis = new List<KanjiDTO>();
            foreach (var id in ids)
            {
                var kanji = await _db.Kanjis.FindAsync(id);
                if (kanji == null)
                    return NotFound();
                else
                    kanjis.Add(KanjiToDTO(kanji));
            }
            return kanjis;
        }

        [HttpGet("{listName}")]
        public async Task<ActionResult<IEnumerable<KanjiDTO>>> GetKanjis(string listName, int startIndex, int endIndex)
        {
            var kanjis = new List<KanjiDTO>();
            
            if (listName == "popular")
            {
                
                for (int i = startIndex; i <= endIndex; i++)
                {
                    var kanji = await _db.Kanjis.FindAsync(i);
                    if (kanji == null)
                        return NotFound();
                    kanjis.Add(KanjiToDTO(kanji));
                }
                return kanjis;
            }
            if (listName == "favorite")
            {
                
                var user = await _db.Users.Include(v => v.Kanjis).FirstOrDefaultAsync(v => v.Username == User.FindFirstValue(ClaimTypes.Name));
                for (int i = startIndex; i <= endIndex; i++)
                {
                    var kanji = user!.Kanjis.FirstOrDefault(v => v.Id == i);
                    if (kanji == null)
                        return NotFound();
                    else
                        kanjis.Add(KanjiToDTO(kanji));
                }
                return kanjis;
            }
            else 
                return NotFound();
        }

        

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Kanji>> PostKanji(KanjiPostDTO kanjiDTO)
        {
            if (string.IsNullOrWhiteSpace(kanjiDTO.Writing) || string.IsNullOrWhiteSpace(kanjiDTO.OnReadings) || string.IsNullOrWhiteSpace(kanjiDTO.KunReadings) || string.IsNullOrWhiteSpace(kanjiDTO.Meaning))
               return BadRequest();
            var kanji = new Kanji { Writing = kanjiDTO.Writing, OnReadings = kanjiDTO.OnReadings, KunReadings = kanjiDTO.KunReadings, Meaning=kanjiDTO.Meaning };
            await _db.Kanjis.AddAsync(kanji);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetKanji), new {id =  kanji.Id}, kanji);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutKanji(int id, Kanji kanji)
        { 
            if (id != kanji.Id)
            {
                return BadRequest();
            }

            _db.Entry(kanji).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) 
            {
                if (!KanjiExists(id).Result)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
                
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteKanji(int id)
        {
            var kanji = await _db.Kanjis.FindAsync(id);
            if (kanji == null)
            {
                return NotFound();
            }

            _db.Kanjis.Remove(kanji);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private static KanjiDTO KanjiToDTO(Kanji kanji)
        {
            return new KanjiDTO { Id = kanji.Id, KunReadings = kanji.KunReadings, Meaning = kanji.Meaning, OnReadings = kanji.OnReadings, Writing = kanji.Writing };
        }

        private async Task<bool> KanjiExists(int id)
        {
            var E = await _db.Kanjis.FindAsync(id);
            if (E == null)
            {
                return false;
            }
            else
            {
                return true;
            }
            
        }
    }
}
