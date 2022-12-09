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
        public async Task<ActionResult<IEnumerable<Shared.Kanji>>> GetKanjis()
        {
            var kanjis = await db.Kanjis.ToListAsync();
            var result = new List<Shared.Kanji>();
            foreach (var item in kanjis)
            {
                char writing = item.Writing[0];

                string meanings = item.Meanings;

                var ons = item.OnReadings.Split('0');
                var onReadings = new List<string>();
                foreach (var on in ons)
                {
                    onReadings.Add(on);
                }

                var kunReadings = new Dictionary<string, string>();
                var kunsAndMeanings = item.KunReadings.Split('0');
                foreach (var p in kunsAndMeanings)
                {
                    var pair = p.Split('1');
                    kunReadings[pair[0]] = pair[1];
                }

                result.Add(new Shared.Kanji(writing, meanings, onReadings, kunReadings) { Id = item.Id});
            }

            return result;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Shared.Kanji>> GetKanji(int id)
        {
            var kanjiModel = await db.Kanjis.FindAsync(id);

            if (kanjiModel == null)
            {
                return NotFound();
            }

            char writing = kanjiModel.Writing[0];
            string meanings = kanjiModel.Meanings;
            var onReadings = new List<string>();
            foreach (var item in kanjiModel.OnReadings.Split('0'))
            {
                onReadings.Add(item);
            }
            var kunReadings = new Dictionary<string, string>();
            foreach (var item in kanjiModel.KunReadings.Split('0'))
            {
                var keyValue = item.Split('1');
                kunReadings[keyValue[0]] = keyValue[1];
            }

            var kanji = new Shared.Kanji(writing, meanings, onReadings, kunReadings) { Id = kanjiModel.Id};
            return kanji;
        }
    }
}
