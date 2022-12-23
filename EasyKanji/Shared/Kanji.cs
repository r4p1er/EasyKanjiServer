using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyKanji.Shared
{
    public class Kanji
    {
        public int Id { get; set; }
        public char Writing { get; set; }
        public string Meaning { get; set; }
        public List<string> OnReadings { get; set; }
        public List<string> KunReadings { get; set; }
        public Dictionary<string, string> Words { get; set; }

        public Kanji(char writing, string meaning, List<string> onReadings, List<string> kunReadings, Dictionary<string, string> words)
        {
            Writing = writing;
            Meaning = meaning;
            OnReadings = onReadings;
            KunReadings = kunReadings;
            Words = words;
        }
    }
}
