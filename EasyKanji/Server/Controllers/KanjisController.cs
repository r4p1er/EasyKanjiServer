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
                char writing = item.Writing[0];

                string meaning = item.Meaning;

                var onItems = item.OnReadings.Split('0');
                var onReadings = new List<string>();
                foreach (var on in onItems)
                {
                    onReadings.Add(on);
                }

                var kunItems = item.KunReadings.Split('0');
                var kunReadings = new List<string>();
                foreach (var kun in kunItems)
                {
                    kunReadings.Add(kun);
                }

                var wordsPairs = item.Words.Split('0');
                var words = new Dictionary<string, string>();
                foreach (var pair in wordsPairs)
                {
                    string key = pair.Split('1')[0];
                    string value = pair.Split('1')[1];
                    words[key] = value;
                }

                result.Add(new Shared.Kanji(writing, meaning, onReadings, kunReadings, words) { Id = item.Id});
            }

            return result;
        }
    }
}
