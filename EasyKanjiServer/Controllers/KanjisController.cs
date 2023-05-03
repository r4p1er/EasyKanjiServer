using EasyKanjiServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasyKanjiServer.Models.DTOs;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Xml.Linq;

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

        [HttpGet("{id:min(1)}")]
        public async Task<ActionResult<KanjiDTO>> GetKanji(int id)
        {
            var kanji = await _db.Kanjis.FindAsync(id);

            if (kanji == null)
            {
                return NotFound(new { errors = "There is no such a kanji." });
            }

            return KanjiToDTO(kanji);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<KanjiDTO>>> GetKanjis([FromQuery]int[] ids)
        {
            var kanjis = new List<KanjiDTO>();

            foreach (var id in ids)
            {
                var kanji = await _db.Kanjis.FindAsync(id);

                if (kanji != null)
                    kanjis.Add(KanjiToDTO(kanji));
            }

            return kanjis;
        }

        [HttpGet("{listName:alpha}")]
        public async Task<ActionResult<IEnumerable<KanjiDTO>>> GetKanjisByListName(string listName, int s = 1, int e = 1)
        {
            if (s > e)
            {
                return BadRequest(new { errors = "Start id can't be greater than end id." });
            }

            var kanjis = new List<KanjiDTO>();
            
            if (listName == "popular")
            {
                for (int i = s; i <= e; i++)
                {
                    var kanji = await _db.Kanjis.FindAsync(i);

                    if (kanji != null)
                        kanjis.Add(KanjiToDTO(kanji));
                }

                return kanjis;
            }

            if (listName == "favorite")
            {
                var user = await _db.Users.Include(v => v.Kanjis).FirstOrDefaultAsync(v => v.Username == User.FindFirstValue(ClaimTypes.Name));

                if (user == null)
                {
                    return BadRequest(new { errors = "Only authorized user can get favorite kanjis." });
                }

                for (int i = s; i <= e; i++)
                {
                    var kanji = user!.Kanjis.FirstOrDefault(v => v.Id == i);

                    if (kanji != null)
                        kanjis.Add(KanjiToDTO(kanji));
                }

                return kanjis;
            }
            
            return NotFound(new { errors = "There is no such a list." });
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<KanjiDTO>>> Search([FromQuery] string q)
        {
            var result = new List<KanjiDTO>();

            foreach (var comma in q.Split(',', StringSplitOptions.TrimEntries))
            {
                foreach (var japaneseComma in comma.Split('、', StringSplitOptions.TrimEntries))
                {
                    foreach (var japaneseSpace in japaneseComma.Split('　', StringSplitOptions.TrimEntries))
                    {
                        if (string.IsNullOrWhiteSpace(japaneseSpace)) continue;

                        foreach (var kanji in await _db.Kanjis.ToListAsync())
                        {
                            bool writing = kanji.Writing.Contains(japaneseSpace, StringComparison.InvariantCultureIgnoreCase);
                            bool meaning = kanji.Meaning.Contains(japaneseSpace, StringComparison.InvariantCultureIgnoreCase);
                            bool kunReadings = string.Concat(kanji.KunReadings.Split('.')).Contains(japaneseSpace, StringComparison.InvariantCultureIgnoreCase);
                            bool onReadings;

                            if (japaneseSpace.Length >= 2 && "ョュウクグスズツヅヌフブムユルオコゴソゾトドノホボモヨロ".Contains(japaneseSpace[japaneseSpace.Length - 2]) && "ウー".Contains(japaneseSpace[japaneseSpace.Length - 1]))
                            {
                                var alteredJapaneseSpace = japaneseSpace[japaneseSpace.Length - 1] == 'ウ' ? japaneseSpace.Substring(0, japaneseSpace.Length - 1) + 'ー' : japaneseSpace.Substring(0, japaneseSpace.Length - 1) + 'ウ';
                                onReadings = kanji.OnReadings.Split(',').Any(x => x.Equals(japaneseSpace, StringComparison.InvariantCultureIgnoreCase) || x.Equals(alteredJapaneseSpace, StringComparison.InvariantCultureIgnoreCase));
                            }
                            else
                            {
                                onReadings = kanji.OnReadings.Split(',').Any(x => x.Equals(japaneseSpace, StringComparison.InvariantCultureIgnoreCase));
                            }

                            if (writing || kunReadings || onReadings || meaning)
                            {
                                result.Add(KanjiToDTO(kanji));
                            }
                        }
                    }
                }
            }

            return result;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<KanjiDTO>> PostKanji(KanjiPostDTO kanjiDTO)
        {
            if (string.IsNullOrWhiteSpace(kanjiDTO.OnReadings) && string.IsNullOrWhiteSpace(kanjiDTO.KunReadings))
                return BadRequest(new { errors = "On readings and kun readings can't be unspecified at the same time." });

            var kanji = await _db.Kanjis.FirstOrDefaultAsync(x => x.Writing == kanjiDTO.Writing);

            if (kanji != null)
            {
                return BadRequest(new { errors = "This kanji already exists." });
            }

            kanji = new Kanji { Writing = kanjiDTO.Writing, OnReadings = kanjiDTO.OnReadings, KunReadings = kanjiDTO.KunReadings, Meaning = kanjiDTO.Meaning };
            await _db.Kanjis.AddAsync(kanji);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetKanji), new {id =  kanji.Id}, kanjiDTO);
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutKanji(int id, KanjiPatchDTO dto)
        { 
            if (string.IsNullOrWhiteSpace(dto.OnReadings) && string.IsNullOrWhiteSpace(dto.KunReadings))
            {
                return BadRequest(new { errors = "On readings and kun readings can't be unspecified at the same time." });
            }

            var kanji = await _db.Kanjis.FindAsync(id);

            if (kanji == null)
            {
                return BadRequest(new { errors = "There is no such a kanji." });
            }

            kanji.Writing = !string.IsNullOrWhiteSpace(dto.Writing) ? dto.Writing : kanji.Writing;
            kanji.KunReadings = !string.IsNullOrWhiteSpace(dto.KunReadings) ? dto.KunReadings : kanji.KunReadings;
            kanji.OnReadings = !string.IsNullOrWhiteSpace(dto.OnReadings) ? dto.OnReadings : kanji.OnReadings;
            kanji.Meaning = !string.IsNullOrWhiteSpace(dto.Meaning) ? dto.Meaning : kanji.Meaning;
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteKanji(int id)
        {
            var kanji = await _db.Kanjis.FindAsync(id);

            if (kanji == null)
            {
                return NotFound(new { errors = "There is no such a kanji." });
            }

            _db.Kanjis.Remove(kanji);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private static KanjiDTO KanjiToDTO(Kanji kanji)
        {
            return new KanjiDTO { Id = kanji.Id, KunReadings = kanji.KunReadings, Meaning = kanji.Meaning, OnReadings = kanji.OnReadings, Writing = kanji.Writing };
        }
    }
}
