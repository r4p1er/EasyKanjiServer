using EasyKanji.Server.Models;
using EasyKanji.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyKanji.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KanjisController : ControllerBase
    {
        private readonly ApplicationContext db;

        public KanjisController(ApplicationContext context) 
        { 
            db = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Shared.Kanji>>> GetKanjis([FromQuery]int fromId = 1, [FromQuery]int count = 1)
        {
            if (fromId < 1 || count < 1) return BadRequest();
            var kanjis = await db.Kanjis.Where(x => x.Id >= fromId && x.Id < fromId + count).ToListAsync();
            var result = new List<Shared.Kanji>();
            foreach (var item in kanjis)
            {
                result.Add(ModelKanjiToSharedKanji(item));
            }

            return result;
        }

        [HttpGet]
        [Route("search")]
        public async Task<ActionResult<IEnumerable<Shared.Kanji>>> Search([FromQuery]string query)
        {
            var items = query.Split('、');
            var kanjis = new List<Shared.Kanji>();
            foreach (var item in items)
            {
                var selection = await db.Kanjis.Where(x => x.Writing == item || x.OnReadings.Contains(item) || x.KunReadings.Contains(item) || x.Meaning.Contains(item) || x.Words.Contains(item)).ToListAsync();
                foreach (var k in selection)
                {
                    kanjis.Add(ModelKanjiToSharedKanji(k));
                }
            }
            return kanjis.Distinct().ToList();
        }

        private Shared.Kanji ModelKanjiToSharedKanji(Models.Kanji kanji)
        {
            char writing = kanji.Writing[0];

            string meaning = kanji.Meaning;

            var onItems = kanji.OnReadings.Split('0');
            var onReadings = new List<string>();
            foreach (var on in onItems)
            {
                onReadings.Add(on);
            }

            var kunItems = kanji.KunReadings.Split('0');
            var kunReadings = new List<string>();
            foreach (var kun in kunItems)
            {
                kunReadings.Add(kun);
            }

            var wordsPairs = kanji.Words.Split('0');
            var words = new Dictionary<string, string>();
            foreach (var pair in wordsPairs)
            {
                string key = pair.Split('1')[0];
                string value = pair.Split('1')[1];
                words[key] = value;
            }

            return new Shared.Kanji(writing, meaning, onReadings, kunReadings, words) { Id = kanji.Id };
        }
    }
}
