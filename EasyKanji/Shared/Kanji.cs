using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyKanji.Shared
{
    public class Kanji
    {
        public char Writing { get; set; }
        public string Meanings { get; set; }
        public List<string> OnReadings { get; set; }
        public Dictionary<string, string> KunReadings { get; set; }

        public Kanji(char writing, string meanings, List<string> onReadings, Dictionary<string, string> kunReadings)
        {
            Writing = writing;
            Meanings = meanings;
            OnReadings = onReadings;
            KunReadings = kunReadings;
        }
    }
}
